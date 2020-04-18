# GottaGoFust
Bunny hop speedrun game

[Discord server](https://discordapp.com/invite/UQ9Ka7u)

## Table of Contents
1. [Pre Conditions](#pre-conditions)
2. [Contribution Guide](#contribution-guide)
3. [Questions](#questions)

## Pre Conditions
To be able to run this projects, you'll need to have a couple of programs installed

Program | Minimal Version | Description
-|-|-
Unity | 2019.1.4f | Game Engine (Development Kit)
Git | 2.0.0 | Version Control


If you want to contribute with coding on this project, you'll need

Program | Minimal Version | Description
-|-|-
Visual Studio or MonoDevelop | Visual Studio Community 2017+ or MonoDevelop 7.5 | Scripting Development Environment

 > Disclaimer: If you're not a Terminal Ninja, I recommend you to use a Git Client like [Github Desktop](https://desktop.github.com/) or the built in Visual Studio git tools. This simplifies the usage of Git through a GUI.

 ## How to Run?
 To get this project running in Unity, you need to follow two simple steps:
1. Clone this repository.
2. Open the project in Unity.

## Contribution Guide
We follow the [Git Flow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) guidelines. This might sound confusing at first, but it's actually a pretty easy way to keep the repository organized.

The **master** is the release branch, this means that it only contains code that is release-ready.

The **develop** branch is the branch where all other branches get merged into. Here everything is tested, and when it is release-ready, it can be merged into **master** through a *release branch*.

Everything else goes into separate branches, the following types of branches are available:

Type | Description
-|-
feature/* | The feature branches are used to add new features from the development branch. So whenever you are working on something new, it should go into a feature branch.
hotfix/* | The hotfix branches are used when working on (bug) fixes, and starts from **master** branch.
release/* | The release branches are always branched off of develop. Whenever a new release is made, it should go through a release branch into **master**.

 > Disclaimer: For any questions regarding how to use Git Flow, check the [questions](#questions) area.

## Questions
For any questions regarding the usage of Git, Git Flow, or Github Desktop send a message to Chillu. It's best to ask this through Discord. Either look me up in our Discord channel, or add me as Chillu#8847.
