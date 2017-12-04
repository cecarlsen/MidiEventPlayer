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
	public struct MidiFileInfo
	{
		public MidiChannelInfo[] channelInfo;


		public void Reset()
		{
			channelInfo = new MidiChannelInfo[16];
			for( int c = 0; c < 16; c++ ) channelInfo[c].Reset();
		}


		public override string ToString()
		{
			System.Text.StringBuilder text = new System.Text.StringBuilder();
			text.AppendLine( "MidiFileInfo" );
			for( int ch = 0; ch < channelInfo.Length; ch++ ) {
				MidiChannelInfo chInfo = channelInfo[ch];
				if( chInfo.isUsed ){
					text.AppendLine( "\tChannel Index " + ch );
					text.AppendLine( "\t\tNote Key Range (" + chInfo.noteKeyMin + ", " + chInfo.noteKeyMax + ")" );
					text.AppendLine( "\t\tNote Vel Range (" + chInfo.noteVelMin + ", " + chInfo.noteVelMax + ")" );
					for( int cc = 0; cc < 128; cc++ ) {
						if( chInfo.IsCCUsed( cc ) ){
							text.AppendLine( "\t\tCC" + cc + " Range (" + chInfo.controlChangeValMin[cc] + ", " + chInfo.controlChangeValMax[cc] + ")" );
						}
					}
				}
			}
			return text.ToString();
		}
	}
}