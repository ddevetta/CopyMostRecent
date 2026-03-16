# Copy Most Recent
## History
I don't have backup software - I usually just periodically copy directories like Projects and Documents to an external SSD using windows copy.
This works ok, until the directories get huge, with hundreds of gigs of data and thousands of files. The vast majority of those files have never changed since the last 'backup' and therefore don't need replacing, so I found myself yearning for a windows copy option similar to this:

<img src="./Images/Replace if more recent.jpg" width="400px" style="border:solid 1px"/>

So it sounded like a good project to take up in my spare time. And here it is. 
It doesn't hack into the 'Replace Files' dialog, but it sort of emulates it by adding an 'Extended' context menu option for Directories called 'Copy if Newer' (Extended context menu entries are activated when the user presses Shift while right-clicking). This will then invoke this tool.
## Goal
I wanted a simple one-directional 'Copy/Replace if Newer' function. I didn't want auto-sync, or 'diff'-style capabilities, just plain 'has this file been modified since it was last copied across, if so update the copy' function.
## Components
The process is performed in three phases - 
- Recursively **Scan** the source and destination directories - all classes for this are in ```DireectoryScan```
- **Compare** the results of the two scans and determine which files need refreshing - classes are in ```DirectoryCompare```
- Perform the **Copy** operation of the ones that have changed - all classes in ```DirectoryCopy```

Results are shown between the Compare and Copy phases asking for confirmation.
'Source/Destination directory' pairs can be saved as Plans. If the context-menu is invoked on Directory for which it is a Source directory in a plan, that plan is automatically loaded.
## Code Highlights
The code demonstrates some cool functionality that may be of interest, viz:
- Asynchronous execution, both through 'async/await' (UI thread) and Task.Run() (new thread).
- Starting two processes and awaiting completion of both 
- Providing feedback to the UI on progress, via the ```IProgress``` interface
- Cancel an async function using a ```CancellationToken```
- Low-level _buffered copy_ where one buffer is written while a new buffer is being read
- Saving user parameter data using an override of the default ```ApplicationSettingsBase``` (see class ```UserOptions```)
- A Setup installer that adds entries to the registry (to activate the Extended Context Menu option)

