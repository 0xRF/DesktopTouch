using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/*
    Credits: 
    Modified Redirection To Work On OSX/Linux & Windows
    https://github.com/Trojaner25/RocketPlus/blob/master/Detour/RedirectionHelper.cs
  
    Original
    https://github.com/sschoener/cities-skylines-detour
*/

 public static class RedirectionHelper
    {
        #if UNITY_EDITOR
        
        private static readonly Dictionary<IntPtr, MethodOffsets> OriginalOffsets = new Dictionary<IntPtr, MethodOffsets>();

        private static unsafe void PatchJumpTo(IntPtr site, IntPtr target)
        {
            if (!OriginalOffsets.ContainsKey(site))
            {
                MethodOffsets offsets = new MethodOffsets();
                byte* numPtr2 = (byte*)site.ToPointer();
                offsets.A = numPtr2[0];
                offsets.B = numPtr2[1];
                offsets.C = numPtr2[10];
                offsets.D = numPtr2[11];
                offsets.E = numPtr2[12];
                offsets.F = *((long*)(numPtr2 + 2));
                OriginalOffsets.Add(site, offsets);
            }
            byte* numPtr = (byte*)site.ToPointer();
            numPtr[0] = 0x49;
            numPtr[1] = 0xbb;
            *((long*)(numPtr + 2)) = target.ToInt64();
            numPtr[10] = 0x41;
            numPtr[11] = 0xff;
            numPtr[12] = 0xe3;
        }

        public static void RedirectCalls(MethodBase from, MethodBase to)
        {
            IntPtr functionPointer = from.MethodHandle.GetFunctionPointer();
            IntPtr target = to.MethodHandle.GetFunctionPointer();
            PatchJumpTo(functionPointer, target);
        }

        public static unsafe void Restore(IntPtr ptr)
        {
            MethodOffsets state = OriginalOffsets[ptr];
            byte* numPtr = (byte*)ptr;
            numPtr[0] = state.A;
            numPtr[1] = state.B;
            *((long*)(numPtr + 2)) = state.F;
            numPtr[10] = state.C;
            numPtr[11] = state.D;
            numPtr[12] = state.E;
        }

        public static void Restore(MethodBase target)
        {
            Restore(target.MethodHandle.GetFunctionPointer());
        }

        private class MethodOffsets
        {
            public byte A;
            public byte B;
            public byte C;
            public byte D;
            public byte E;
            public long F;
        }
        
        #endif
    }