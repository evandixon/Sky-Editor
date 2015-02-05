So, you want to make an extension for Sky Editor?  Here's how.
If you're using a copy of this project, solution and all, you might not need to do the following.
If you just copied the vb files (which is perfectly acceptable), you need to do the following.

1. Reference Sky Editor Base.exe, disabling Copy Local in My Project (you don't HAVE to disable Copy Local, but it's unneccessary for it to be enabled.)
2. In My Project/Compile, make sure that the dll from this class library is copied into Sky Editor's ~/Resources/Plugins/ in order for Sky Editor to load it.
	If you do not, you will need to manually put it there in order for Sky Editor to load it.
3. Ensure the assembly name ends in _plg, because plugins may have external dlls, there needs to be a distinction.
4. Modify the code files with your code, following the instructions and tips given by the comments.