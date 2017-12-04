/*
	Created by Carl Emil Carlsen.
	Copyright 2017 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MidiEventPlayer))]
[CanEditMultipleObjects]
public class MidiEventPlayerInspector : Editor
{
	SerializedProperty _midiFile;
	SerializedProperty _midiFileTempo;
	SerializedProperty _playOnAwake;
	SerializedProperty _onNoteOnEvent;
	SerializedProperty _onNoteOffEvent;
	SerializedProperty _onControlChangeEvent;
	SerializedProperty _onProgramChangeEvent;
	SerializedProperty _onAftertouchEvent;
	SerializedProperty _onChannelPressureEvent;
	SerializedProperty _onLoadedEvent;
	SerializedProperty _onPlayEvent;
	SerializedProperty _onPauseEvent;
	SerializedProperty _eventsFold;


	void OnEnable()
	{
		_midiFile = serializedObject.FindProperty( "_midiFile" );
		_midiFileTempo = serializedObject.FindProperty( "_midiFileTempo" );
		_playOnAwake = serializedObject.FindProperty( "_playOnAwake" );
		_onNoteOnEvent = serializedObject.FindProperty( "_onNoteOnEvent" );
		_onNoteOffEvent = serializedObject.FindProperty( "_onNoteOffEvent" );
		_onControlChangeEvent = serializedObject.FindProperty( "_onControlChangeEvent" );
		_onProgramChangeEvent = serializedObject.FindProperty( "_onProgramChangeEvent" );
		_onAftertouchEvent = serializedObject.FindProperty( "_onAftertouchEvent" );
		_onChannelPressureEvent = serializedObject.FindProperty( "_onChannelPressureEvent" );
		_onLoadedEvent = serializedObject.FindProperty( "_onLoadedEvent" );
		_onPlayEvent = serializedObject.FindProperty( "_onPlayEvent" );
		_onPauseEvent = serializedObject.FindProperty( "_onPauseEvent" );
		_eventsFold = serializedObject.FindProperty( "_eventsFold" );
	}


	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField( _midiFile );
		EditorGUILayout.PropertyField( _midiFileTempo );
		EditorGUILayout.PropertyField( _playOnAwake );
		_eventsFold.boolValue = EditorGUILayout.Foldout( _eventsFold.boolValue, "Events" );
		if( _eventsFold.boolValue ){
			EditorGUILayout.PropertyField( _onNoteOnEvent );
			EditorGUILayout.PropertyField( _onNoteOffEvent );
			EditorGUILayout.PropertyField( _onControlChangeEvent );
			EditorGUILayout.PropertyField( _onProgramChangeEvent );
			EditorGUILayout.PropertyField( _onAftertouchEvent );
			EditorGUILayout.PropertyField( _onChannelPressureEvent );
			EditorGUILayout.PropertyField( _onLoadedEvent );
			EditorGUILayout.PropertyField( _onPlayEvent );
			EditorGUILayout.PropertyField( _onPauseEvent );
		}
			
		serializedObject.ApplyModifiedProperties();
	}
}