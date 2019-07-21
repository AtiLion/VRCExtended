# VRCExtended
This mod offers you quality of life improvements in the game VRChat. It adds features that allow the user to have a much better experience in the game.

> **Disclaimer:**
> This mod uses [VRCModLoader](https://github.com/Slaynash/VRCModLoader) to work properly, so you will need to install that first.
>  
>  **Warning:**
>  The VRChat team is not very keen on modding or reverse engineering the game, while the mod does not include anything that would ruin the fun for others, using it may still be a bannable offence.
>   
>  **USE IT AT YOUR OWN RISK**, I am not responsible for any bans or any punishments you may get by using this mod!

## Installation ##
Please make sure you have downloaded and installed [VRCModLoader](https://github.com/Slaynash/VRCModLoader) before continuing!
> 1. Download the latest **Release** version of the mod from the [Releases](https://github.com/AtiLion/VRCExtended/releases) section
> 2. Navigate to your VRChat directory *(where all the VRChat game files are located)*
> 3. Drag the DLL file from the downloaded mod ZIP file into the Mods folder
> 4. That's it! Now just run the game and the mod should be installed!

## Features ##
If you wish to suggest more features that can improve everyone's lives feel free to suggest them.
> - Ask use portal *[This feature asks you if you wish to enter the portal before you do]*
> - Anti-crasher *[This feature removes any elements that could crash you]*
> - Local colliders *[This features let's you interact with other avatar's colliders]*
>   - Others have local colliders *[You see when a user interacts with another user's colliders]*
>   - Others can touch you *[You can see when someone interacts with your colliders]*
>   - Target only hand colliders *[Only the hand colliders are able to interact with other colliders]*
>   - Fake colliders on self *[Allows you to interact with colliders even if you don't have them]*
>   - Fake colliders on others *[Allows others to interact with colliders even if they don't have them]*
> - Refresh user *[This feature let's you refresh the user info instantly]*
> - Refresh social groups *[This feature let's you refresh the entire social screen]*

## Anti-crasher Information ##
The anti-crasher configuration file is located inside your VRChat directory under the name "antiCrash.json"
> - PolyLimit = Limit the amount of polygons that a mesh can have
> - MaxPolygons = How many polygons can be in a single mesh before it is removed
> - ParticleLimit = Limit the amount of particles an avatar can give off
> - MaxParticles = How many particles an avatar can give off
> - ShaderBlacklist = Remove shaders based on a blacklist
> - BlacklistedShaders = The partial names of shaders that are removed if blacklist is enabled

## Contributing ##
I will try to be as active as I can in adding features and fixing bugs but I can't do everything alone, so I ask the community to also help out. Listed below are how you can help the project continue forward.
> **Developers:**
> Feel free to add features you may believe to be useful to other users, or if you wish you can add features that other users have request that I have not gotten to yet.
> 
> **Users:**
> You can contribute by reporting any bugs caused by the mod, as well as requesting features that you think might be useful to others as well as you.

## Known bugs ##
 - You have to login every time you turn on the game *(This is an issue with the mod loader)*
 - Refresh doesn't work instantly *(This is an issue in how VRChat's API caching works)*

## FAQ ##
If you have any more questions feel free to ask them in the [Issues](https://github.com/AtiLion/VRCExtended/issues) section.
> - How do I make it so I don't have to login every time? *You can try using the [AutoRelog mod by Slaynash](https://github.com/Slaynash/AutoRelog)*
> - How do I report a bug or suggest a feature? *You can create an issue report in the [Issues](https://github.com/AtiLion/VRCExtended/issues) section*
> - How do I get the debug version of the mod? *Simply install the **Debug** version of the mod found in the [Releases](https://github.com/AtiLion/VRCExtended/releases) section*
> - Can I suggest a feature that may affect how other's play the game? *No, any requested feature that affects the gameplay of others or is considered unfair will be ignored*
> - Can I get banned using this mod? *Yes, if any of the VRChat team finds out that you are using mods you are most likely going to be banned, so I suggest you don't go advertising it*
> - Does this mod work in VR? *Yes, the mod is designed to work fully with VR as well as desktop*
> - Avatars are taking longer to load with local colliders. *An avatar with a ton of colliders will take longer to load as it has to register all of them*
> - Sometimes all of the colliders go weird. *If an avatar has a world sized bone collider on it, it may cause things to start and look weird as the collider interacts with all of yours*
> - My game takes a while to close. *Use the [QuitFix mod by HerpDerpinstine](https://github.com/HerpDerpinstine/QuitFix)*

## How to compile ##
 1. Make sure you have a program that can compile C# code
 2. Create a folder called *"lib"*
 3. Make sure the lib folder contains the following files:
	- Assembly-CSharp.dll
	- Assembly-CSharp-firstpass.dll
	- Newtonsoft.Json.dll
	- UnityEngine.AudioModule.dll
	- UnityEngine.CoreModule.dll
	- UnityEngine.dll
	- UnityEngine.ParticleSystemModule.dll
	- UnityEngine.PhysicsModule.dll
	- UnityEngine.TextRenderingModule.dll
	- UnityEngine.UI.dll
	- UnityEngine.UIModule.dll
	- UnityEngine.AnimationModule.dll
	- VRCCore-Standalone.dll
	- VRCSDK2.dll
	- VRCModLoader.dll
	- VRCTools.dll
4. Use your compiler to compile the code