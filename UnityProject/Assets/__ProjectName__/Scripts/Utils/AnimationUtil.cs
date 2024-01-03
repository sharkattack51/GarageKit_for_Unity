using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class AnimationUtil
    {
        public static void RewindReset(Animation anim)
        {
            anim.Rewind();
            anim.Play();
            anim.Sample();
            anim.Stop();
        }
    }
}
