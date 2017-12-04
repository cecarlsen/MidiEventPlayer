using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Midi.Visualisation;

public class StandardMidiEnvelopeTest : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField] int _channelIndex = 2;
	[SerializeField] int _noteCapacity = 64;
	[SerializeField] GameObject _noteVizPrefab;
	[SerializeField] StandardMidiEnvelope _envelopeSetting;

	// We keep a pool of envelopes to avoid continuous instantiation.
	MidiEnvelope[] _envelopePlayerPool;

	// We keep track of which keys are on and what active envelopes they refer to.
	Dictionary<byte,MidiEnvelope> _envelopPlayerLookup = new Dictionary<byte,MidiEnvelope>();

	// We keep a pool of visual objects to avoid continuous instantiation.
	Transform[] _noteVizPool;
	List<KeyValuePair<Transform,MidiEnvelope>> _noteVizEnvPlaPairs = new List<KeyValuePair<Transform,MidiEnvelope>>();

	// Will contain range (min, max) values for normalization.
	SmfLite.MidiChannelInfo _channelInfo;


	void Awake()
	{
		// Prepare pools.
		_envelopePlayerPool = new MidiEnvelope[_noteCapacity];
		_noteVizPool = new Transform[_noteCapacity];
		for( int e = 0; e < _envelopePlayerPool.Length; e++ ) {
			_envelopePlayerPool[e] = new StandardMidiEnvelope();
			_noteVizPool[e] = Instantiate( _noteVizPrefab ).transform;
			_noteVizPool[e].SetParent( transform );
			_noteVizPool[e].gameObject.SetActive( false );
		}
	}


	void Update()
	{
		// Loop backwards so that we can remove finished envelopes.
		for( int i = _noteVizEnvPlaPairs.Count-1; i >= 0; i-- ){
			KeyValuePair<Transform,MidiEnvelope> pair = _noteVizEnvPlaPairs[i];
			if( pair.Value.isActive ){
				// Evaluate envelope and update visualisation.
				pair.Key.localScale = Vector3.one * pair.Value.Evaluate();
				pair.Key.Translate( Vector3.up * Time.deltaTime * 5 );
			} else {
				// Envelope has finised, now remove it from the active list.
				pair.Key.gameObject.SetActive( false );
				_noteVizEnvPlaPairs.RemoveAt( i );
			}
		}
	}


	public void OnNoteOn( int channeIndex, byte rawKey, byte rawVel )
	{
		if( channeIndex != _channelIndex ) return;

		// Try to find an inactive envelope.
		MidiEnvelope env = System.Array.Find( _envelopePlayerPool, e => !e.isActive );
		if( env == null ) return;

		// Try to find an inactive visualisation object.
		Transform noteViz = System.Array.Find( _noteVizPool, n => !n.gameObject.activeSelf );
		if( !noteViz ) return;

		// Copy envelope settings.
		env.CopyFrom( _envelopeSetting );

		// Start the envelope.
		env.NoteOn();

		// Use the channel info object to normalize and map incoming midi values.
		float x = Mathf.InverseLerp( _channelInfo.noteKeyMin, _channelInfo.noteKeyMax, rawKey ) * 2 - 1;

		// Apply to visualisation object and activate.
		noteViz.position = Vector3.right * x * 3;
		noteViz.gameObject.SetActive( true );

		// Store in active list.
		_noteVizEnvPlaPairs.Add( new KeyValuePair<Transform,MidiEnvelope>( noteViz, env ) );
		_envelopPlayerLookup.Add( rawKey, env );
	}


	public void OnNoteOff( int channeIndex, byte rawKey )
	{
		if( channeIndex != _channelIndex ) return;

		// Try finding active envelope that corresponds to key.
		MidiEnvelope env;
		if( _envelopPlayerLookup.TryGetValue( rawKey, out env ) ){
			// Request the envelope to finish.
			env.NoteOff();
			// Remove from lookup so that the same key can be pressed again.
			_envelopPlayerLookup.Remove( rawKey );
		}
	}


	public void OnLoaded( TextAsset file, SmfLite.MidiFileInfo info )
	{
		_channelInfo = info.channelInfo[_channelIndex];
	}
}