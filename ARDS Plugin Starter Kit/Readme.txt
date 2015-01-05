So, you want to make a code generator for Sky Editor?  Here's how.
If you're using a copy of this project downloaded from Codeplex, you might not need to do the following.
If you just copied MyCodeGenerator.vb (which is perfectly acceptable), you need to do the following.

1. Reference Sky Editor Base.exe, disabling Copy Local in My Project
2. If you're making a code for any Pokemon Mystery Dungeon game supported by Sky Editor, you also need to reference Resources/Plugins/Sky Editor.dll
3. In My Project/Compile, make sure that the dll from this class library is copied into Sky Editor's ~/Resources/Plugins/ in order for Sky Editor to load it.
	If you do not, you will need to manually put it there in order for Sky Editor to load it.
4. Make sure the assembly name ends in _plg.dll, otherwise Sky Editor won't load it.  This is so you can include any dependencies in the Plugins directory and have Sky Editor not try to load it.
5. Modify MyCodeGenerator.vb, following the advice of the plugins.

If you like C# better, look up a converter for VB to C#.  As long as you configure the project correctly, it doesn't matter what .Net language you use (I mean VB vs. C#, I can't say for sure about others).