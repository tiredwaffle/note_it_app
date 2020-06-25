# note_it_app
[University project] Android app for saving all kind of notes, links, events, ets (Igor Sikorsky Kyiv Polytechnic Institute, OOP course)
Project is a program implementation of the application-organizer for smartphones running on the Android operating system. The implementation of the program part is written in C #, and for the interface the Xamarin.Forms tool is used, which allows you to create a user-defined interface for a set of visual elements described in the XAML markup language that is displayed in the visual elements of the corresponding operating system (Android, iOS and Windows Phone).

## Technical details
Implemented on C#.
SQLite is used to store the added data - lightweight relational database management system.  SQLite stores the entire database (including definitions, tables, indexes, and data) in a single standard file on the machine on which the application is running. Ease of implementation is achieved because before the transaction, the entire file that stores the database, is blocked. 
To make the app to interact with the basic features of the smartphone the Xamarin.Essentials container is used. This tool gives access among others to the state of Accelerometer, App Information, Barometer, Battery, Clipboard, Color Converters, Compass, Connectivity, Detect Shake, Device Display Information, Device Information, Email, File System Helpers, Flashlight, Geocoding, Geolocation, Gyroscope, Launcher, Magnetometer, Main Thread, Maps, Open Browser, Orientation Sensor, Phone Dialer, Platform Extensions (Size, Rect, Point), Preferences, Secure Storage, Share, SMS, Text-to-Speech, Unit Converters, Version Tracking, Vibrate. 
This particular application implements the ability to open pages in the default browser, share notes or switch to Dialer. 
The Xamarin.Forms tool was used to implement the interface. All visual elements are taken from standard libraries. All graphics for this project work were taken from open sources or were performed independently in the vector-graphic editor CorelDRAW.

## Product description
Launching the product on the smartphone, the user opens a list of notes of type "Text note". Navigating through the menu with tabs the user then opens the list "Contacts", "Weblinks" or "Events".
The interface of each of the main pages has an "Add" button, which should be clicked in order to create a new note of a given type. This creates a new window in which the user specifies the name of the note and specific properties depending on the type (text in "Text note", link in "Weblink", form for recording details about a person in "Contacts" and start and end dates in "Events"). The same page opens when you select an already created note to edit.
For "Text Note" notes, changes are saved permanently, for other types when the user click "Save". Also on the information change pages for "Contacts" the option "Call" is available, and for "Weblinks" - "Open". Available sorting options and search. Sorting notes by date of creation and title is available on the main pages of the lists.
The user can also share notes of each type on social networks, send it as an email, or via SMS.
The InfoPage page contains information about the developer and the application.
