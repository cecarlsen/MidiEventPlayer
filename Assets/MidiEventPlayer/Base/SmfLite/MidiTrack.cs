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
	/// <summary> 
	/// Midi track.
	/// Stores only one track (usually a MIDI file contains one or more tracks).
	/// </summary>
	public class MidiTrack
	{
		List<KeyValuePair<int,MidiEvent>> _sequence;

		public readonly int totalDelta;
		public readonly MidiChannelInfo[] channelInfo = new MidiChannelInfo[16];


		public MidiTrack( List<KeyValuePair<int,MidiEvent>> sequence )
		{
			_sequence = sequence;
			for( int c = 0; c < 16; c++ ) channelInfo[c].Reset();

			// Gather information.
			foreach( KeyValuePair<int,MidiEvent> pair in _sequence )
			{
				totalDelta += pair.Key;
			
				MidiEvent.Type type;
				int chI;
				pair.Value.UnpackStatus( out type, out chI );
			
				if( !channelInfo[chI].isUsed ) channelInfo[chI].isUsed = true;

				switch( type ){
				case MidiEvent.Type.NoteOn:
					byte key = pair.Value.data1;
					byte vel = pair.Value.data2;
					if( key < channelInfo[chI].noteKeyMin ) channelInfo[chI].noteKeyMin = key;
					if( key > channelInfo[chI].noteKeyMax ) channelInfo[chI].noteKeyMax = key;
					if( vel < channelInfo[chI].noteVelMin ) channelInfo[chI].noteVelMin = vel;
					if( vel > channelInfo[chI].noteVelMax ) channelInfo[chI].noteVelMax = vel;
					break;
				case MidiEvent.Type.ControlChange:
					int cc = pair.Value.data1;
					byte val = pair.Value.data2;
					if( val < channelInfo[chI].controlChangeValMin[cc] ) channelInfo[chI].controlChangeValMin[cc] = val;
					if( val > channelInfo[chI].controlChangeValMax[cc] ) channelInfo[chI].controlChangeValMax[cc] = val;
					break;
				// TODO the rest
				}
			}
		}


		// Returns an enumerator which enumerates the all delta-event pairs.
		public List<KeyValuePair<int,MidiEvent>>.Enumerator GetEnumerator()
		{
			return _sequence.GetEnumerator();
		}


		public KeyValuePair<int,MidiEvent> GetAtIndex( int index )
		{
			return _sequence[index];
		}


		public override string ToString()
		{
			var s = "";
			foreach( var pair in _sequence ) s += pair;
			return s;
		}
	}
}