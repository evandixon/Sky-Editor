So, you want to make an extension for Sky Editor?  Here's how.
If you're using a copy of this project downloaded from Codeplex, you might not need to do the following.
If you just copied the vb files (which is perfectly acceptable), you need to do the following.

1. Reference Sky Editor Base.exe, disabling Copy Local in My Project
2. In My Project/Compile, make sure that the dll from this class library is copied into Sky Editor's ~/Resources/Plugins/ in order for Sky Editor to load it.
	If you do not, you will need to manually put it there in order for Sky Editor to load it.
3. Modify the code files with your code, following the instructions and tips given by the comments.

If you like C# better, look up a converter for VB to C#.  As long as you configure the project correctly, it doesn't matter what .Net language you use (I mean VB vs. C#, I can't say for sure about others).