# Sound Controller for Unity3D

This is a sound controller. The main idea of this plugin is to not use raw AudioSource, but have a nice wrapper around it from which you can easily control what sounds at what time you want to play.
The reason I made this plugin is that I don't really want to pay money for Audiotoolkit from unity asset store, and I didn't really liked it when I had the chance to use it.

![alt text][screen]

[screen]: https://github.com/dimixar/audio-controller-unity/blob/master/screenshot.PNG

Main Features:
- Organize sounds by categories
- Add tags to your sounds to filter them even further
- Add multiple variants of the same sound and easily play at random.
- Change Audioclips, edit category names, and sound names from play mode without the fear of losing those changes
- All data is saved on a scriptable object asset
- You can create multiple data assets and swap them easily
- Create whole cues of sounds with ease (very useful when you have dialog Voice Over)
- Add Random Pitch to sound items
- Add Random Volume to sound items
- It's not a singleton!!!

How to add it to your project and scene:
1. Extract the latest .unitypackage file found in releases (link here: https://github.com/dimixar/audio-controller-unity/releases/tag/0.0.1);
2. Create new SoundControllerData asset using the contextual menu from Project hierarchy;
3. Find the prefab with SoundController in OSSC folder and drag it onto the scene;
4. Add your new created asset into the corresponding object field of the SoundContoller component;
5. Select gameobject with SoundController and start adding categories and items.

How to use it:
- Check the example Scene for that.
