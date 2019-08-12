//////////////////////////////////////////////////////
// MK Glow Change Settings Legacy        			//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de | www.michaelkremmel.store //
// Copyright © 2017 All rights reserved.            //
//////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.Glow.Example
{
    public class ChangeSettingsLegacy : MonoBehaviour
    {
        private Legacy.MKGlow _mkGlow;

        private void Awake()
        {
            _mkGlow = GetComponent<Legacy.MKGlow>();
        }

        //Main
        public int debugView
        { 
            get { return (int)_mkGlow.debugView; }
            set { _mkGlow.debugView = (DebugView)value; }
        }

        //Bloom
        public float bloomThreshold
		{ 
			get { return _mkGlow.bloomThreshold.minValue; }
			set { _mkGlow.bloomThreshold.minValue = value; }
		}
        public float bloomClamp
		{ 
			get { return _mkGlow.bloomThreshold.maxValue; }
			set { _mkGlow.bloomThreshold.maxValue = value; }
		}
        public float bloomScattering
		{ 
			get { return _mkGlow.bloomScattering; }
			set { _mkGlow.bloomScattering = value; }
		}
        public float bloomIntensity
		{ 
			get { return _mkGlow.bloomIntensity; }
			set { _mkGlow.bloomIntensity = value; }
		}
    }
}