# 2D/3D Ground Readme

## Unity Requirements

This area will include information relative to the specific example and in most cases will require you to make some Unity adjustments for the example to work as intended. We could have set this up in an automatic fashion but we didn't want to modify or import example files that would override your project settings - so you will have to adjust some minor settings to get the examples to work!

### Layer Requirements

* This example uses different layer properties within Unity. This is different then a layermask - we are relying on layers and the ability for Unity to filter the physics system associated with those layers.
  * The Scriptable Object being used '../ScriptableObjects/Raycast_Ground_1m' needs to be updated or adjusted to account for a 'Ground' layer in your Unity settings.
* We didn't include these layers in this example folder because we didn't want to overwrite your already existing layers: *fun fact*, Unity is limited to 32 layers (of which they use 0,1,2,4,& 5 - so really you only have **27** to customize) and they are being stored as an 'int': so if you have a layer at position 8 and we decided to bring in our project settings for layers, we would overwrite all of your layer related project settings for position 8 which isn't cool!
* If you're unfamiliar with layers and/or how to create your own: please see this [Unity Reference Documentation](https://docs.unity3d.com/2022.2/Documentation/Manual/Layers.html) for more information on layers in Unity.

### Input Requirements

* This example has a two scripts that are using the Old Unity Input System
  * 'CameraExample.cs'
    * Mouse inputs that are rotating the blue capsule on a fixed pivot point. This is active in this example but we are doing nothing with it right now in this example - but it is used in other examples.
  * 'SimpleJump.cs'
    * When the user hits the space bar it will apply an impulse force to the overall red capsule.

### Example Details

* When you hit play, hit the space bar to watch the capsule jump. The raycast visual is very small so you might not be able to see that but it's there on the bottom of the capsule facing down. The 'SimpleJump.cs' script is using the raycaster information to limit when you can 'jump' by storing information associated with the collision via a small distance ray below the capsule and just changing a boolean '_OnGround' from true to false based on if the raycaster comes back with a confirmed 'ground hit'
