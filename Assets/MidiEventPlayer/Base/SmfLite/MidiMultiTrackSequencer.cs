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
	public class MidiMultiTrackSequencer
	{
		MidiTrackSequencer[] _sequencers;

		public readonly MidiFileInfo info;

		bool _isPlaying;

		public bool isPlaying { get { return _isPlaying; } }


		public MidiMultiTrackSequencer( MidiFileContainer song, float midiFileTempo )
		{
			_sequencers = new MidiTrackSequencer[ song.tracks.Count ];

			info.Reset();

			for( int s = 0; s < _sequencers.Length; s++ ){
				MidiTrackSequencer sq = new MidiTrackSequencer( song.tracks[s], song.division, midiFileTempo );
				MidiTrack tr = sq.track;
				_sequencers[s] = sq;

				// Gather file info.
				for( int c = 0; c < 16; c++ ){
					if( tr.channelInfo[c].isUsed ) info.channelInfo[c].isUsed = true;
					if( tr.channelInfo[c].noteKeyMin < info.channelInfo[c].noteKeyMin ) info.channelInfo[c].noteKeyMin = tr.channelInfo[c].noteKeyMin;
					if( sq.track.channelInfo[c].noteKeyMax > info.channelInfo[c].noteKeyMax ) info.channelInfo[c].noteKeyMax = sq.track.channelInfo[c].noteKeyMax;
					if( tr.channelInfo[c].noteVelMin < info.channelInfo[c].noteVelMin ) info.channelInfo[c].noteVelMin = tr.channelInfo[c].noteVelMin;
					if( tr.channelInfo[c].noteVelMax > info.channelInfo[c].noteVelMax ) info.channelInfo[c].noteVelMax = tr.channelInfo[c].noteVelMax;
					for( int cc = 0; cc < 128; cc++ ) {
						if( tr.channelInfo[c].controlChangeValMin[cc] < info.channelInfo[c].controlChangeValMin[cc] ) info.channelInfo[c].controlChangeValMin[cc] = tr.channelInfo[c].controlChangeValMin[cc];
						if( tr.channelInfo[c].controlChangeValMax[cc] > info.channelInfo[c].controlChangeValMax[cc] ) info.channelInfo[c].controlChangeValMax[cc] = tr.channelInfo[c].controlChangeValMax[cc];
					}
				}
			}
		}


		public List<MidiEvent> Start( float startTime = 0.0f )
		{
			List<MidiEvent> events = new List<MidiEvent>();
			foreach( MidiTrackSequencer sq in _sequencers ){
				List<MidiEvent> sqEvents = sq.Start( startTime );
				if( sqEvents != null ) events.AddRange( sqEvents );
			}
			_isPlaying = true;
			return events;
		}


		public List<MidiEvent> Advance( float deltaTime )
		{
			List<MidiEvent> events = new List<MidiEvent>();
			int playingCount = 0;
			foreach( MidiTrackSequencer sq in _sequencers ){
				List<MidiEvent> sqEvents = sq.Advance( deltaTime );
				if( sqEvents != null ) events.AddRange( sqEvents );
				if( sq.isPlaying ) playingCount++;
			}
			if( playingCount == 0 ) _isPlaying = false;
			return events;
		}
	}
}