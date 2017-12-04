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
	/// Sequencer for MIDI tracks.
	/// Works like an enumerator for MIDI events.
	/// Note that not only Advance() but also Start() can return MIDI events.
	/// </summary>
	public class MidiTrackSequencer
	{
		MidiTrack _track;
		List<KeyValuePair<int,MidiEvent>>.Enumerator _enumerator;
		bool _isPlaying;
		float _pulsePerSecond;
		float _pulseToNext;
		float _pulseCounter;

		public bool isPlaying { get { return _isPlaying; } }
		public MidiTrack track { get { return _track; } }

		// Constructor
		//   "ppqn" stands for Pulse Per Quater Note,
		//   which is usually provided with a MIDI header.
		public MidiTrackSequencer( MidiTrack track, int ppqn, float bpm )
		{
			_track = track;
			_pulsePerSecond = bpm / 60.0f * ppqn;
		}

		// Start the sequence.
		// Returns a list of events at the beginning of the track.
		public List<MidiEvent> Start( float startTime = 0.0f )
		{
			_enumerator.Dispose();
			_enumerator = _track.GetEnumerator();
			_pulseCounter = 0;

			if( _enumerator.MoveNext() ){
				_pulseToNext = _enumerator.Current.Key;
				_isPlaying = true;
				return Advance( startTime );
			} else {
				_isPlaying = false;
				return null;
			}
		}

		// Advance the song position.
		// Returns a list of events between the current position and the next one.
		public List<MidiEvent> Advance( float deltaTime )
		{
			if( !_isPlaying ) return null;

			_pulseCounter += _pulsePerSecond * deltaTime;

			if( _pulseCounter < _pulseToNext ) return null;
			
			var messages = new List<MidiEvent>();

			while( _pulseCounter >= _pulseToNext ){
				var pair = _enumerator.Current;
				messages.Add( pair.Value );
				if( !_enumerator.MoveNext () ){
					_isPlaying = false;
					break;
				}

				_pulseCounter -= _pulseToNext;
				_pulseToNext = _enumerator.Current.Key;
			}

			return messages;
		}
	}
}