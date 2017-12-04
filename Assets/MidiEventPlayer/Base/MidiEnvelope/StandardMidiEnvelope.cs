//
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

namespace Midi.Visualisation
{
	[System.Serializable]
	public class StandardMidiEnvelope : MidiEnvelope
	{
		public float attack = 0.05f;
		public float decay = 0.1f;
		[Range(0,1)] public float sustain = 0.7f;
		public float release = 1f;
		public bool useSmoothStep = false;

		float _noteTime;
		float _activeAttack;
		float _activeDecay;
		float _activeSustain;
		float _activeRelease;
		float _valueAtNoteOff;


		public override void NoteOn()
		{
			_activeAttack = attack;
			_activeDecay = decay;
			_activeSustain = sustain;
			_activeRelease = release;

			_isActive = true;
			_phase = Phase.Attack;
			_noteTime = Time.time;
			_value = _activeAttack > 0 ? 0 : 1;
		}


		public override void NoteOff()
		{
			_phase = Phase.Release;
			_noteTime = Time.time;
			_valueAtNoteOff = _value;
		}


		public override float Evaluate()
		{
			// Only update once per frame.
			if( Time.frameCount == _lastUpdatedFrame ) return _value;

			// Idle case.
			if( _phase == Phase.Idle ) return 0;

			// Sustain case.
			if( _phase == Phase.Sustain ) return _value;

			// Update
			float time = Time.time - _noteTime;
			if( _phase == Phase.Attack ){
				if( time < _activeAttack ){
					_value = time / _activeAttack;
					if( useSmoothStep ) _value = Mathf.SmoothStep( 0, 1, _value );
				} else if( _activeDecay > 0 && time - _activeAttack < _activeDecay ){
					_phase = Phase.Decay;
					if( useSmoothStep )  _value = Mathf.SmoothStep( 1, _activeSustain, (time-_activeAttack) / _activeDecay );
					else _value = 1 - ( (time-_activeAttack) / _activeDecay ) * ( 1 - _activeSustain );
				} else {
					_phase = Phase.Sustain;
					_value = _activeSustain;
				}
			} else if( _phase == Phase.Decay ){
				time -= _activeAttack;
				if( time < _activeDecay ){
					if( useSmoothStep ) _value = Mathf.SmoothStep( 1, _activeSustain, time / _activeDecay );
					else _value = 1 - ( time / _activeDecay ) * ( 1 - _activeSustain );
				} else {
					_phase = Phase.Sustain;
					_value = _activeSustain;
				}
			} else { // Phase.Release
				if( time < _activeRelease ){
					if( useSmoothStep ) _value = Mathf.SmoothStep( _valueAtNoteOff, 0, time / _activeRelease );
					else _value = _valueAtNoteOff - _valueAtNoteOff * ( time / _activeRelease );
				} else {
					_value = 0;
					_phase = Phase.Idle;
					_isActive = false;
				}
			}

			_lastUpdatedFrame = Time.frameCount;
			return _value;
		}



		public override void CopyFrom( MidiEnvelope env )
		{
			if( !( env is StandardMidiEnvelope ) ) return;

			StandardMidiEnvelope sme = env as StandardMidiEnvelope;

			attack = sme.attack;
			decay = sme.decay;
			sustain = sme.sustain;
			release = sme.release;
			useSmoothStep = sme.useSmoothStep;
		}
	}
}