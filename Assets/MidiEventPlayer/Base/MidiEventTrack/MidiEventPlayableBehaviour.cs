// Copyright (C) 2017 Carl Emil Carlsen
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.Timeline
{
	
	[System.Serializable]
	public class MidiEventPlayableBehaviour : PlayableBehaviour
	{
		public MidiEventPlayer player;
		public TextAsset clip;
		public float tempo;


		public override void OnBehaviourPlay( Playable playable, FrameData info )
		{
			if( !player || !Application.isPlaying ) return;

			// Assign clip to player.
			if( clip && ( player.midiFile != clip || !player.isLoaded) ) player.Load( clip, tempo );

			// Pause if timeline is not playing.
			bool isTimelinePlaying = playable.GetGraph().IsPlaying();
			if( !isTimelinePlaying ) player.Pause();
		}


		public override void OnBehaviourPause( Playable playable, FrameData info )
		{
			if( !player || !Application.isPlaying ) return;

			if( player && player.isPlaying ) player.Pause();
		}


		public override void PrepareFrame( Playable playable, FrameData info )
		{
			if( !player || !Application.isPlaying ) return;

			// Play if timeline is playing and player is not playing
			bool isTimelinePlaying = playable.GetGraph().IsPlaying();
			float clipTime = (float) playable.GetTime();
			if( isTimelinePlaying ){
				if( !player.isPlaying ){
					player.time = clipTime;
					player.Play();
				}
			} else { // Scrub.
				player.time = clipTime;
			}
		}
	}
}