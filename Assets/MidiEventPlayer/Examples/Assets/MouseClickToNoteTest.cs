using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickToNoteTest : MonoBehaviour
{
	[SerializeField] int _channelIndex = 0;
	[SerializeField] int _keyIndex = 64;
	[SerializeField] MidiEventPlayer.NoteOnEvent _noteOnEvent;
	[SerializeField] MidiEventPlayer.NoteOffEvent _noteOffEvent;


	void Update()
	{
		if( Input.GetMouseButtonDown(0) ) _noteOnEvent.Invoke( _channelIndex, (byte) _keyIndex, 127 );
		else if( Input.GetMouseButtonUp(0) ) _noteOffEvent.Invoke( _channelIndex, (byte) _keyIndex );
	}
}