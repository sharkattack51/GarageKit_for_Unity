using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Docs : MonoBehaviour
{
    [Multiline(30)]
    public string discription = @"
/**********
GarageKit for Unity Doc
===

# What is GarageKit ?
GarageKit is a Unity C# framework.
It provides a template scene with a state transition system,
some managers and state scripts, utility scripts and sample scenes,
and a directory structure for your project.

- Current Version
2021+   GarageKit_for_unity2021.unitypackage

- Beginners Tutorial
You can use the Beginners Tutorial to learn how to use packages,
the development flow, and the basic concept of programming.

- Scripts Reference
For information on using scripts in packages, see the Scripts Reference.
**********/
";

    public string openUrl = "https://sharkattack51.github.io/garagekit_doc/";


    void Awake()
    {

    }

    void Start()
    {
        Button btn = this.gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() => {
            Application.OpenURL(openUrl);
        });
    }

    void Update()
    {

    }
}
