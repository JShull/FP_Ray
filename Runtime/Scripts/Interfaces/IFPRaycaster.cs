using System;
using System.Collections;
using System.Collections.Generic;
using FuzzPhyte.Ray;
using UnityEngine;

namespace FuzzPhyte.Ray
{
    public interface IFPRaycaster
    {
        event EventHandler<FP_RayArgumentHit> OnFPRayFireHit;
        event EventHandler<FP_RayArgumentHit> OnFPRayEnterHit;
        event EventHandler<FP_RayArgumentHit> OnFPRayExit;

        event EventHandler<FP_RayArgument> OnFPRayActivate;
        event EventHandler<FP_RayArgument> OnFPRayDeactivate;
    }
}
