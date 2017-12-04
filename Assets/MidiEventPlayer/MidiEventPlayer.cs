//
// MidiEventPlayer.cs - A midi event player based on SmfLite by Keijiro Takahashi.
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
using UnityEngine.Events;
using SmfLite;

public class MidiEventPlayer : MonoBehaviour
{
	[SerializeField] TextAsset _midiFile;
	[SerializeField] float _midiFileTempo = 120;
	[SerializeField] bool _playOnAwake;

	[SerializeField] NoteOnEvent _onNoteOnEvent;
	[SerializeField] NoteOffEvent _onNoteOffEvent;
	[SerializeField] ControlChangeEvent _onControlChangeEvent;
	[SerializeField] AftertouchEvent _onAftertouchEvent;
	[SerializeField] ProgramChangeEvent _onProgramChangeEvent;
	[SerializeField] ChannelPressureEvent _onChannelPressureEvent;
	//[SerializeField] PitchBendEvent _onPitchBendEvent; // TODO
	[SerializeField] LoadedEvent _onLoadedEvent;
	[SerializeField] PlayEvent _onPlayEvent;
	[SerializeField] PauseEvent _onPauseEvent;

	[SerializeField][HideInInspector] bool _eventsFold;

	MidiMultiTrackSequencer _multiSequencer;

	bool _isLoaded;
	bool _isPlaying;
	float _time;

	bool[,] _onMemo = new bool[16,128];

	[System.Serializable] public class NoteOnEvent : UnityEvent<int,byte,byte> {}			// channelIndex, key, velocity
	[System.Serializable] public class NoteOffEvent : UnityEvent<int,byte> {}				// channelIndex, key
	[System.Serializable] public class ControlChangeEvent : UnityEvent<int,byte,byte> {}	// channelIndex, controlIndex, value
	[System.Serializable] public class AftertouchEvent : UnityEvent<int,byte,byte> {}		// channelIndex, key, value
	[System.Serializable] public class ProgramChangeEvent : UnityEvent<int,byte> {}			// channelIndex, instrumentIndex
	[System.Serializable] public class ChannelPressureEvent : UnityEvent<int,byte> {}		// channelIndex, value
	//[System.Serializable] public class PitchBendEvent : UnityEvent<int,float,float> {} // TODO
	[System.Serializable] public class LoadedEvent : UnityEvent<TextAsset,MidiFileInfo> {}
	[System.Serializable] public class PlayEvent : UnityEvent {}
	[System.Serializable] public class PauseEvent : UnityEvent {}

	/// <summary>
	/// Gets the midi file.
	/// </summary>
	public TextAsset midiFile { get { return _midiFile; } }

	/// <summary>
	/// Gets a value indicating whether this a file is loaded.
	/// </summary>
	public bool isLoaded { get { return _isLoaded; } }

	/// <summary>
	/// Gets a value indicating whether the player is playing.
	/// </summary>
	public bool isPlaying { get { return _isPlaying; } }

	/// <summary>
	/// Gets or sets the time.
	/// </summary>
	public float time {
		get { return _time; }
		set {
			_time = value;
			if( _isPlaying ) Play();
		}
	}

	/// <summary>
	/// Gets the note on event.
	/// </summary>
	public NoteOnEvent onNoteOnEvent { get { return _onNoteOnEvent; } }

	/// <summary>
	/// Gets the note off event.
	/// </summary>
	public NoteOffEvent onNoteOffEvent { get { return _onNoteOffEvent; } }

	/// <summary>
	/// Gets the control change event.
	/// </summary>
	public ControlChangeEvent onControlChangeEvent { get { return _onControlChangeEvent; } }

	/// <summary>
	/// Gets the aftertouch event.
	/// </summary>
	public AftertouchEvent onAftertouchEvent { get { return _onAftertouchEvent; } }

	/// <summary>
	/// Gets the program change event.
	/// </summary>
	public ProgramChangeEvent onProgramChangeEvent { get { return _onProgramChangeEvent; } }

	/// <summary>
	/// Gets the channel pressure event.
	/// </summary>
	public ChannelPressureEvent onChannelPressureEvent { get { return _onChannelPressureEvent; } }

	/// <summary>
	/// Gets the loaded event.
	/// </summary>
	/// <value>The on loaded event.</value>
	public LoadedEvent onLoadedEvent { get { return _onLoadedEvent; } }

	/// <summary>
	/// Gets the play event.
	/// </summary>
	public PlayEvent onPlayEvent { get { return _onPlayEvent; } }

	/// <summary>
	/// Gets the pause event.
	/// </summary>
	public PauseEvent onPauseEvent { get { return _onPauseEvent; } }


	void Awake()
	{
		if( _playOnAwake && _midiFile ){
			Load( _midiFile );
			Play();
		}
	}

	
	void Update()
	{
		if( !_isPlaying ) return;

		DispatchEvents( _multiSequencer.Advance( Time.deltaTime ) );

		_isPlaying = _multiSequencer.isPlaying;
		
		_time += Time.deltaTime;
	}


	/// <summary>
	/// Load the specified midiFile with tempo (bpm).
	/// </summary>
	public void Load( TextAsset midiFile, float bpm = -1 )
	{
		if( _isPlaying ){
			_multiSequencer = null;
			_isPlaying = false;
		}

		if( bpm != -1 ) _midiFileTempo = bpm;

		_midiFile = midiFile;
		MidiFileContainer song = MidiFileLoader.Load( _midiFile.bytes );
		_multiSequencer = new MidiMultiTrackSequencer( song, _midiFileTempo );
		_isLoaded = true;

		MidiFileInfo info = _multiSequencer.info;
		_onLoadedEvent.Invoke( midiFile, info );
	}


	/// <summary>
	/// Play at specified time.
	/// </summary>
	public void Play( float time = -1 )
	{
		//if( _sequencers == null ){
		if( _multiSequencer == null ){
			Debug.LogWarning( "Can't play, you need to load a file first.\n" );
			return;
		}

		if( time != -1 ){
			_time = time;
		}

		// Fire note off messages.
		for( int ch = 0; ch < 16; ch++ ) {
			for( int k = 0; k < 128; k++ ) {
				if( _onMemo[ch,k]){
					_onNoteOffEvent.Invoke( ch, (byte) k );
					_onMemo[ch,k] = false;
				}
			}
		}

		_isPlaying = true;
		List<MidiEvent> events = _multiSequencer.Start( _time );
		if( _time == 0 ) DispatchEvents( events );

		_onPlayEvent.Invoke();
	}


	/// <summary>
	/// Pause.
	/// </summary>
	public void Pause()
	{
		_isPlaying = false;
		_onPauseEvent.Invoke();
	}



	void DispatchEvents( List<MidiEvent> events )
	{
		if( events == null ) return;

		foreach( MidiEvent e in events )
		{
			int channelIndex;
			MidiEvent.Type type;
			e.UnpackStatus( out type, out channelIndex );

			switch( type )
			{
			case MidiEvent.Type.NoteOff:
				_onMemo[channelIndex,e.data1] = false;
				_onNoteOffEvent.Invoke( channelIndex, e.data1 );
				break;
			case MidiEvent.Type.NoteOn:
				_onMemo[channelIndex,e.data1] = true;
				_onNoteOnEvent.Invoke( channelIndex, e.data1, e.data2 );
				break;
			case MidiEvent.Type.Aftertouch: _onAftertouchEvent.Invoke( channelIndex, e.data1, e.data2 ); break;
			case MidiEvent.Type.ControlChange: _onControlChangeEvent.Invoke( channelIndex, e.data1, e.data2 ); break;
			case MidiEvent.Type.ProgramChange: _onProgramChangeEvent.Invoke( channelIndex, e.data1 ); break;
			case MidiEvent.Type.ChannelPressure: _onChannelPressureEvent.Invoke( channelIndex, e.data1 ); break;
			case MidiEvent.Type.PitchBend: break; // TODO
			}
		}
	}
}