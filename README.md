# This tool was deprecated on 21/12/2021 - The new version is now written in Go, smaller and faster, it is the new official CLI, you can find the updated repository by [clicking here](https://github.com/HRKings/gitgud-cli)

# GitGud CLI [Deprecated]

[![GitGud](https://img.shields.io/badge/GitGud-v1.0-red?style=flat-square)](https://github.com/HRKings/GitGud/tree/stable)

This repository is crossplatform CLI (Command Line Interface) for the [GitGud](https://github.com/HRKings/GitGud/tree/stable) modular git model. It contains a series of commands that help you use git more quickly and following the model.

- [GitGud CLI](#gitgud-cli)
	- [Installation](#installation)
	- [Usage](#usage)
		- [Commit Module](#commit-module)
			- [Quickadd commit](#quickadd-commit)
			- [Plain commit](#plain-commit)
			- [Full commit](#full-commit)
			- [Lint](#lint)
			- [Generate](#generate)
		- [Flow Module](#flow-module)
			- [Full Init](#full-init)
			- [Init](#init)
			- [Start](#start)
			- [Publish](#publish)
			- [Complete](#complete)
	- [Compiling](#compiling)
	- [Contributing](#contributing)

## Installation

Just download and drop the latest release into a folder, add it to your path and call the executable in your terminal of preference.

## Usage

Using it is rather simple, after you have it on your path you can call it using: `gitgud <module> <args>`

The commands are divided into modules, just like the original model, the ones available at the moment are:

### Commit Module

This module is equivalent to the [Commit submodel](https://github.com/HRKings/GitGud/blob/stable/Git/Commit.md) and is the the default module so you can use the commands directly, without calling the module first.

#### Quickadd commit

This command tracks all files (using `git add .`) and commits the changes using the provided message, it will ask you to select the commit tag and flags via a arrow based menu.

```Bash
gitgud commit quick "Commit subject"
gitgud commit q "Commit subject"
```

_Obs.: You can use the `-f` or `--full` flag to generate a commit with body and footer_

_Obs.: You can use the `--amend` flag to amend your previous comment_

#### Plain commit

This command will perform a simple commit ('git commit -m'), like the above it will ask you for the tag and flags.

```Bash
gitgud commit plain "Commit subject"
gitgud commit p "Commit subject"
```

_Obs.: You can use the `-f` or `--full` flag to generate a commit with body and footer_

_Obs.: You can use the `--amend` flag to amend your previous comment_

#### Plain commit

This command will perform an add commit ('git commit -am'), like the above it will ask you for the tag and flags.

```Bash
gitgud commit add "Commit subject"
gitgud commit a "Commit subject"
```

_Obs.: You can use the `-f` or `--full` flag to generate a commit with body and footer_

_Obs.: You can use the `--amend` flag to amend your previous comment_

#### Lint

This command is for linting commits, if you pass a commit message, it will validate the message and write a report on it explaining errors and warnings.

```Bash
gitgud commit lint "Commit message"
gitgud commit l "Commit message"
```

_Obs.: You can use the `-f` or `--full` flag to generate a commit with body and footer_

_Obs.: You can use the `--amend` flag to amend your previous comment_

#### Generate

This command will generate a commit message based on your inputs (tag and flags).

```Bash
gitgud commit generate "Commit subject"
gitgud commit g "Commit subject"
```

### Flow Module

The Flow module is a wrapper to the [Flow submodel](https://github.com/HRKings/GitGud/blob/master/Flow/GitGud_Flow.md), and its commands are equivalent to the [how to](https://github.com/HRKings/GitGud/blob/master/Flow/GitGud_Flow_HowTo.md) provided in the model.

#### Full Init

This command is for when you just created a project and need to initialize the repository. It will create a normal git repository, then will create and commit an initial commit on the master, and derive a stable branch from it.

```Bash
gitgud flow fullinit
```

#### Init

This command is for when you just already have a repository and wants it to follow the GitGud model, it will just create an stable branch off the master.

```Bash
gitgud flow init
```

#### Start

This command will start a working branch for you to change to your hearts content. It will ask your for a working type and then branch off the master branch.

```Bash
gitgud flow start "branch-name"
```

#### Publish

This command is for when you are working locally and need to push your branch in the internet. When provided a branch type and name, will push the branch to the origin.

```Bash
gitgud flow publish "type/branch-name"
```

#### Complete

This command will merge the provided working branch into the master one and delete the original working branch, but will not do anything if the branch is in the origin, to encourage the use of pull requests for more organization. 

If the working branch is a hotfix type, it will also merge into the stable branch before deletion.

```Bash
gitgud flow complete "type/branch-name"
```

## Compiling

You are welcome to clone and compile this repository. For this you will need .NET 5 on the latest version and compile it from the terminal using:

```Bash
dotnet build -c Release
```

## Contributing

You are more than welcome to contribute to this repository opening issues and pull requests, just remember to follow the specifications of the GitGud model, as this repository follows all of it (obviously).
