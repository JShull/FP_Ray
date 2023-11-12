# Fuzzphyte Unity Tools

## Raycaster

FP_Raycaster is designed and built to wrap the UnityEngine Physics and UnityEngine Physics2D for one simple raycaster to rule them all! It's based on a humble design pattern utilizing a scriptable object pipeline to pass the various raycasting details as well as some internal delegate functions to help keep it clean. There is a way to listen for different events tied to these raycasters as I have provided some custom Argument classes. You only need to know the object that's doing the raycast and you can subscribe to that objects callbacks to do whatever it is you need to do. Please see the Samples/SamplesURP for how this effectively works. I've also included some visual debugging information but its currently tied to the FP_RayMono class - I will eventually get this removed and added as a separate utility. If you're not using the URP pipeline just modify the materials to be standard and you should be good to go.

## Setup & Design

FP_Raycaster is a blend of a using scriptable objects to establish the type of raycaster, then a humble pattern utilizing C# interfaces combined with custom arguments that work with a delegate to allow for listening and invoking those various events.

### Software Architecture

There are currently Four parts to the RP_Raycaster tool. Main Humble Class, Interfaces for setup and use, Scriptable Objects, and a Delegate with a custom argument for subscribing to events.

* FP_Raycaster is the meat of this package and contains all of the code for how the raycasts are operating behind the scenes.
  * You can do a lot with this but for the most part it's really automated to allow you to write minimal code on the Unity Monobehaviour side.
  * You just need to provide the Scriptable Object that represents the type of raycaster you want, an origin, and a destination. You then have to call a SetupRaycaster function via the interface to pass back your instance of the FP_Raycaster class: keeping it humble :100:
    * For how you deal with the delegate callbacks.
    * Subscribe to:
    * >
      > 'OnFPRayFireHit'
      >
      > 'OnFPRayEnterHit'
      >
      > 'OnFPRayExit'
      >
      > 'OnFPRayActivate'
      >
      > 'OnFPRayDeactivate'
* Two Interfaces: IFPRaycaster and IFPRaySetup
  * IFPRaySetup just keeps me honest and needs to be used by whatever MonoBehaviour class you decide to create
  * IFPRaycaster is the glue for the Humble pattern and connects the 5 main raycasting states
    * Activation
    * OnEnter
    * OnFire (stay)
    * OnExit
    * Deactivation
* Delegate and callbacks with arguments: FP_RayArgument and FP_RayArgumentHit. A set of classes that derive from System.EventArgs that provide a nicer way to get information back in the delegate callback functions.
* In general the [Humble pattern](https://www.youtube.com/watch?v=3O_rpTWdGps) is based on some of the work by Jason Weimann.

* In the samples: see the Raycast2DURP and the Raycast3DURP scenes. I've included a quick dummy camera script to allow you to test the different 3D raycasts and the 2D raycasts. The more important example script is going to be the 'FP_RayMono' as it showcases exactly how to setup the FP_Raycaster for use.

### Ways to Extend

* You should be able to easily implement this within a prefab approach combined with some sort of raycast manager that's listening for all of these callbacks or use the callbacks as you see fit .
* Rewrite your own humble pattern from this design and just get rid of this package entirely :smile:
  * Most of the core physics raycast code is based on the very standard use cases - you can generate and derive from the base and override all the functions if you wanted to extend this pattern to other sort of different raycasting needs and/or wanted to add more functionality to it.

## Dependencies

Please see the [package.json](./package.json) file for more information.

## License Notes

* This software running a dual license
* Most of the work this repository holds is driven by the development process from the team over at Unity3D :heart: to their never ending work on providing fantastic documentation and tutorials that have allowed this to be born into the world.
* I personally feel that software and it's practices should be out in the public domain as often as possible, I also strongly feel that the capitalization of people's free contribution shouldn't be taken advantage of.
  * If you want to use this software to generate a profit for you/business I feel that you should equally 'pay up' and in that theory I support strong copyleft licenses.
  * If you feel that you cannot adhere to the GPLv3 as a business/profit please reach out to me directly as I am willing to listen to your needs and there are other options in how licenses can be drafted for specific use cases, be warned: you probably won't like them :rocket:

### Educational and Research Use MIT Creative Commons

* If you are using this at a Non-Profit and/or are you yourself an educator and want to use this for your classes and for all student use please adhere to the MIT Creative Commons License
* If you are using this back at a research institution for personal research and/or funded research please adhere to the MIT Creative Commons License
  * If the funding line is affiliated with an [SBIR](https://www.sbir.gov) be aware that when/if you transfer this work to a small business that work will have to be moved under the secondary license as mentioned below.

### Commercial and Business Use GPLv3 License

* For commercial/business use please adhere by the GPLv3 License
* Even if you are giving the product away and there is no financial exchange you still must adhere to the GPLv3 License

## Contact

* [John Shull](mailto:the.john.shull@gmail.com)
