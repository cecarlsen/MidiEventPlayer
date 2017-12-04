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
using UnityEngine.Video;

namespace UnityEngine.Timeline
{
	[System.Serializable]
	public class MidiEventPlayable : PlayableAsset, ITimelineClipAsset
	{
		//public ExposedReference<TextAsset> clip;
		[SerializeField,NotKeyable] public TextAsset clip;
		[SerializeField,NotKeyable] public float tempo = 120;

		public ClipCaps clipCaps { get { return ClipCaps.None; } }


		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner )
		{
			ScriptPlayable<MidiEventPlayableBehaviour> playable = ScriptPlayable<MidiEventPlayableBehaviour>.Create( graph );

			MidiEventPlayableBehaviour behaviour = playable.GetBehaviour();

			behaviour.clip = clip;//clip.Resolve( graph.GetResolver() );
			behaviour.tempo = tempo;

			return playable;
		}
	}
}