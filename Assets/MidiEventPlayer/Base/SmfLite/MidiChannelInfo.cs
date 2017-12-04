//
// SmfLite.cs - A minimal toolkit for reading standard MIDI files (SMF) in Unity
//
// Original work: Copyright (C) 2013 Keijiro Takahashi
//		https://github.com/keijiro/smflite
//
// Modified work: Copyright (C) 2017 Carl Emil Carlsen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using System.Collections.Generic;

namespace SmfLite
{
	public struct MidiChannelInfo
	{
		public bool isUsed;
		public byte noteKeyMin;
		public byte noteKeyMax;
		public byte noteVelMin;
		public byte noteVelMax;
		public byte[] controlChangeValMin;
		public byte[] controlChangeValMax;

		public bool IsCCUsed( int cc ){ return controlChangeValMin[cc] != 127; }


		public void Reset()
		{
			isUsed = false;
			noteKeyMin = 127;
	        noteKeyMax = 0;
			noteVelMin = 127;
	        noteVelMax = 0;
			controlChangeValMin = new byte[128];
			controlChangeValMax = new byte[128];
			for( int cc = 0; cc < 128; cc++ ) {
				controlChangeValMin[cc] = 127;
			}
		}
	}
}