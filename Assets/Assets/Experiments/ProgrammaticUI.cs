using System.Collections;
using System.Collections.Generic;
using Experiments;
using UnityEngine;
using UnityEngine.UI;

public class ProgrammaticUI : MonoBehaviour {
    private Canvas _canvas;
    
    // Start is called before the first frame update
    void Start() {
        UIFactory.Init(new GFXAdapter());
        _canvas = FindObjectOfType<Canvas>();

        var window = new UGUIResizableWindow(_canvas, new Rect(100, 300, 200, 200));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
