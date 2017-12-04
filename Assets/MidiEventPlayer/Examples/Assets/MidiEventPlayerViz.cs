using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidiEventPlayerViz : MonoBehaviour
{
	[SerializeField] RawImage _noteRawImage;
	[SerializeField] RawImage _controlChangeRawImage;
	[SerializeField] Text _noteText;
	[SerializeField] Text _controlChangeText;
	[SerializeField] Text _aftertouchText;
	[SerializeField] Text _programChangeText;
	[SerializeField] Text _channelPressureText;
	[SerializeField] bool _logFileInfo;
	[SerializeField] bool _logNotes;
	[SerializeField] bool _logControlChanges;
	[SerializeField] bool _logAftertouch;
	[SerializeField] bool _logProgramChanges;
	[SerializeField] bool _logChannelPressure;

	float[,] _notes;
	Color32[] _notePix;
	Texture2D _noteTex;

	float[,] _controlChanges;
	Color32[] _controlChangePix;
	Texture2D _controlChangeTex;

	Text[] _texts;
	float[] _lastEventTimes;

	string prependLog { get { return "<b>[" + GetType().Name + "]</b> "; } }

	const int chCount = 16;
	const int numCount = 127; // Keys, control change numbers and so on...
	const float textFadeDuration = 2f;


	void Awake()
	{
		_notes = new float[chCount,numCount];
		_notePix = new Color32[chCount*numCount];
		_noteTex = CreateTexture( "NoteTex" );

		_controlChanges = new float[chCount,numCount];
		_controlChangePix = new Color32[chCount*numCount];
		_controlChangeTex = CreateTexture( "CCTex" );

		_lastEventTimes = new float[5];
		for( int i = 0; i < _lastEventTimes.Length; i++ ) _lastEventTimes[i] = -99;
		_texts = new Text[]{ _noteText, _controlChangeText, _aftertouchText, _programChangeText, _channelPressureText };

		if( _noteRawImage ) _noteRawImage.texture = _noteTex;
		if( _controlChangeRawImage ) _controlChangeRawImage.texture = _controlChangeTex;
	}


	void OnEnable()
	{
		Reset();
	}


	void LateUpdate()
	{
		for( int c = 0; c < chCount; c++ ){
			float chT = c / (chCount-1f);
			for( int k = 0; k < numCount; k++ ){
				int p = c*numCount+k;
				_notePix[p] = Color.HSVToRGB( chT, 1, _notes[c,k] );
				_controlChangePix[p] = Color.HSVToRGB( chT, 1, _controlChanges[c,k] );
			}
		}
		_noteTex.SetPixels32( _notePix );
		_controlChangeTex.SetPixels32( _controlChangePix );
		_noteTex.Apply();
		_controlChangeTex.Apply();

		for( int t = 0; t < _texts.Length; t++ ) UpdateTextFade( t );
	}



	public void OnLoaded( TextAsset file, SmfLite.MidiFileInfo info )
	{
		if( _logFileInfo ){
			string text = "Loaded Midi File '" + file.name + "'\n" + info;
			Debug.Log( text );
		}
	}


	public void OnNoteOn( int channelIndex, byte rawKey, byte rawVelocity )
	{
		_notes[channelIndex,(int)rawKey] = ( (int) rawVelocity ) / (float) numCount;
		string text = "NoteOn. ch: " + channelIndex + ", key: " + rawKey + ", vel: " + rawVelocity;
		if( _noteText ) _noteText.text = text;
		if( _logNotes ) Debug.Log( text + "\n" );
		_lastEventTimes[0] = Time.time;
	}


	public void OnNoteOff( int channelIndex, byte rawKey )
	{
		_notes[channelIndex,(int)rawKey] = 0;
		string text = "NoteOff. ch: " + channelIndex + ", key: " + rawKey;
		if( _noteText ) _noteText.text = text;
		if( _logNotes ) Debug.Log( text + "\n" );
		_lastEventTimes[0] = Time.time;
	}


	public void OnControlChange( int channelIndex, byte rawControlIndex, byte rawValue )
	{
		_controlChanges[channelIndex,(int)rawControlIndex] = ( (int) rawValue ) / (float) numCount;
		string text = "ControlChange. ch: " + channelIndex + ", control: " + rawControlIndex + ", val: " + rawValue;
		if( _controlChangeText ) _controlChangeText.text = text;
		if( _logControlChanges ) Debug.Log( text + "\n" );
		_lastEventTimes[1] = Time.time;
	}


	public void OnAftertouch( int channelIndex, byte rawKey, byte rawValue )
	{
		string text = "Aftertouch. ch: " + channelIndex + ", key: " + rawKey + ", val: " + rawValue;
		if( _aftertouchText ) _aftertouchText.text = text;
		if( _logAftertouch ) Debug.Log( text + "\n" );
		_lastEventTimes[2] = Time.time;
	}


	public void OnProgramChange( int channelIndex, byte rawInstrumentIndex )
	{
		string text = "ProgramChange. ch: " + channelIndex + ", instrument: " + rawInstrumentIndex;
		if( _programChangeText ) _programChangeText.text = text;
		if( _logProgramChanges ) Debug.Log( text + "\n" );
		_lastEventTimes[3] = Time.time;
	}


	public void OnChannelPressure( int channelIndex, byte rawPressureValue )
	{
		string text = "ChannelPressure. ch: " + channelIndex + ", pressure: " + rawPressureValue;
		if( _channelPressureText ) _channelPressureText.text = text;
		if( _logChannelPressure ) Debug.Log( text + "\n" );
		_lastEventTimes[4] = Time.time;
	}


	void Reset()
	{
		for( int c = 0; c < chCount; c++ ){
			for( int k = 0; k < numCount; k++ ){
				_notes[c,k] = 0;
				_controlChanges[c,k] = 0;
			}
		}
		if( _noteText ) _noteText.text = "";
		if( _controlChangeText ) _controlChangeText.text = "";
		if( _aftertouchText ) _aftertouchText.text = "";
		if( _programChangeText ) _programChangeText.text = "";
		if( _channelPressureText ) _channelPressureText.text = "";
	}


	Texture2D CreateTexture( string name )
	{
		Texture2D tex = new Texture2D( numCount, chCount, TextureFormat.ARGB32, false );
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.name = name;
		return tex;
	}


	void UpdateTextFade( int i )
	{
		float timeElapsed = Time.time - _lastEventTimes[i];
		if( timeElapsed > textFadeDuration ) return;

		if( _texts[i] ) _texts[i].color = Color.Lerp( Color.white, Color.clear, timeElapsed / textFadeDuration );
	}
}