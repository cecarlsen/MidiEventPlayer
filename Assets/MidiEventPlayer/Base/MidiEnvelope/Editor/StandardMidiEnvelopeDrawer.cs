/*
	Created by Carl Emil Carlsen.
	Copyright 2017 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk
*/

using UnityEngine;
using UnityEditor;

namespace Midi.Visualisation
{
	[CustomPropertyDrawer(typeof(StandardMidiEnvelope))]
	public class StandardMidiEnvelopeDrawer : PropertyDrawer
	{
		const float envelopeVizHeight = 100;


		public override void OnGUI( Rect rect, SerializedProperty prop, GUIContent label )
		{
			if( !prop.isExpanded ){
				prop.isExpanded = EditorGUI.Foldout( rect, prop.isExpanded, prop.displayName );
				return;
			}


			// Using BeginProperty & EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty( rect, label, prop );


			// Get properties.
			SerializedProperty attack = prop.FindPropertyRelative( "attack" );
			SerializedProperty decay = prop.FindPropertyRelative( "decay" );
			SerializedProperty sustain = prop.FindPropertyRelative( "sustain" );
			SerializedProperty release = prop.FindPropertyRelative( "release" );
			SerializedProperty useSmoothStep = prop.FindPropertyRelative( "useSmoothStep" );


			Rect drawRect = new Rect( rect );
			drawRect.height = EditorGUIUtility.singleLineHeight;

			prop.isExpanded = EditorGUI.Foldout( drawRect, prop.isExpanded, prop.displayName );
			drawRect.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField( drawRect, attack );
			drawRect.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField( drawRect, decay );
			drawRect.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField( drawRect, sustain );
			drawRect.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField( drawRect, release );
			drawRect.y += EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField( drawRect, useSmoothStep );
			drawRect.y += EditorGUIUtility.singleLineHeight;

			// Ensure sanity.
			attack.floatValue = Mathf.Max( attack.floatValue, 0 );
			decay.floatValue = Mathf.Max( decay.floatValue, 0 );
			sustain.floatValue = Mathf.Clamp01( sustain.floatValue );
			release.floatValue = Mathf.Max( release.floatValue, 0 );

			// Visualize.

			drawRect.height = envelopeVizHeight;

			float totalDuration = attack.floatValue + decay.floatValue + release.floatValue;

			EditorGUI.DrawRect( drawRect, Color.black );

			Vector3 thisPoint = new Vector3();
			Vector3 lastPoint = new Vector3();

			bool hasAttack = attack.floatValue > 0;
			bool hasDecay = decay.floatValue > 0;
			bool hasRelease = release.floatValue > 0;

			Handles.color = Color.red;

			if( hasAttack ){
				lastPoint.Set( drawRect.xMin, drawRect.yMax, 0 );
				thisPoint.Set( drawRect.xMin + ( attack.floatValue / totalDuration ) * drawRect.width, drawRect.yMin, 0 );
				Handles.DrawLine( lastPoint, thisPoint );
				lastPoint = thisPoint;
			} else {
				float y = hasDecay ? drawRect.yMin : drawRect.yMax - sustain.floatValue * drawRect.height;
				lastPoint.Set( drawRect.xMin, y, 0 );
			}

			Handles.color = Color.yellow;

			if( hasDecay || ( !hasAttack && !hasRelease ) ){
				float xOff = ( !hasDecay ? 1 : ( decay.floatValue / totalDuration ) ) * drawRect.width;
				thisPoint.Set( lastPoint.x + xOff, drawRect.yMax - sustain.floatValue * drawRect.height, 0 );
				Handles.DrawLine( lastPoint, thisPoint );
				lastPoint = thisPoint;
			}

			Handles.color = Color.green;

			if( hasRelease ){
				thisPoint.Set( lastPoint.x + ( release.floatValue / totalDuration ) * drawRect.width, drawRect.yMax, 0 );
				Handles.DrawLine( lastPoint, thisPoint );
				lastPoint = thisPoint;
			}

			EditorGUI.EndProperty();
		}


		public override float GetPropertyHeight( SerializedProperty prop, GUIContent label )
		{
			return prop.isExpanded ? EditorGUI.GetPropertyHeight( prop ) + envelopeVizHeight : EditorGUIUtility.singleLineHeight;
		}
	}
}