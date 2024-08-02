# BlazingNotes

After searching for weeks for a note-taking app that fulfills my needs I decided to write my own.  
For all the people that like Post-Its because of their simplicity but still want something digital :)


## In which ways is it different?

### Your thoughts - your data

Everything is saved locally. No big company that crawls your data and trains some AI models with it.

### No hierarchies

Is your brain organized in hierarchies? No? Why should you organize your thoughts and notes in hierarchies then?  
In BlazingNotes there is a simple textbox where you can put in what's in your brain right now! No need to navigate to obscure directories upfront!

### No bureaucracy

You know some apps, but they require too many clicks and metadata until you can enter the important things?  
As just mentioned, it's mainly a simple textbox and nothing else! 

### Blazing Search

How do you search for new recipe ideas? Or ways to fix a software or hardware bug?  
I know how: you just put some words into your favorite search engine. 

Let's have the same for your notes and thoughts!  
And because all your data is stored locally it is blazing fast! You will probably find the information faster than in your brain ;)

### No stress

BlazingNotes is here to free your mind and give you the good feeling that the information is there when you need it again.  
There are no endless list of todos and documents that make you feel overwhelmed.

Some info is obsolete? Delete it!  
But it could be important in some months? Then just archive it!  
You deleted some note by accident? Calm down! It's in the trash for 30 days!

### Tags are great

Just like your brain remembers something with songs and smells, we use #tags.  
With the right #tags you can give your notes - and your productivity - some superpowers!

### Productivity

Helpful metadata and queries to not stand in your way.  
You are in a meeting and just want to write something down? And have no time to use #tags?  
No problem - there is an "Untagged" query, and with the automatically tracked creation timestamp you know when you wrote it!

### and more to come!

Trust me, I have some cool ideas!  
And I won't forget them, because they are in BlazingNotes - my second brain - already ;)


## Run it

Installers will come soon - in the meantime you have to clone the repo and run it manually.  
You also need to have the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

1. clone the repo
2. execute `scripts/update-database.cmd` (with the repository root as working directory)
2. navigate into the directory `/src/BlazingNotes/`
3. run `dotnet run`
4. open the browser on https://localhost:8401 (developer certificate needed) or alternatively on http://localhost:8400
5. start typing :)