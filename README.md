# STasks for Unity

STasks is a simple library that aims at replacing most common uses of coroutines. It's easy to use, faster to write and performance-first.

## Installation

Import [the UnityPackage](https://github.com/MartinIsla/STasks/blob/master/STasks.unitypackage) into your project and you're good to go.

## Usage

### Perform a task after some time has passed
This is probably the most common case for coroutines.

```c#
private IEnumerator DoSomethingAfterThreeSeconds () 
{
    yield return new WaitForSeconds(3);
    Debug.Log("Do Something");
}
```

With STasks, you can make this easily using `STasks.Do(action, time)`:
```c#
STasks.Do(() => Debug.Log("Do Something"), after: 3.0f)
```

> Print "do something" after 3 seconds

### Perform a task repeatedly forever
Unity provides a way of invoking a method repeatedly every a given number of seconds.

```c#
InvokeRepeating("PrintDoSomething", 1.0f, 3.0f);
```
>After one second, start printing "do something" every three seconds.

The biggest problem with this is the method has to be passed as a string, meaning that the method isn't tracked. Using STasks, you can use `STasks.DoRepeating(action, every, startAfter)`. 

```c#
STasks.DoRepeating(() => Debug.Log("Do Something"), every: 3.0f, startAfter: 1.0f);
```
>After one second, start printing "do something" every three seconds.

### Perform a task repeatedly after a condition is met
Using coroutines, you can use loops to perform a task indefinitely after a certain condition is met.
```c#
private IEnumerator TakeDamageSlowly () 
{
    int currentLives = 10;
    while (currentLives > 0)
    {
        currentLives--;
        yield return new WaitForSeconds(1);
    }
}
```
> Take a life from the player every second until they're dead

With STasks, you can do this using `STasks.DoUntil(action, condition, every)`
```c#
int currentLives = 10;
STasks.DoUntil(action: () => currentLives--, condition: () => currentLives == 0, every: 1.0f);
```
Please note the "every" parameter is optional. It has a value of 0 by default, meaning the action will be executed every frame.

### Perform a task when a condition is met
Coroutines can also be used to wait until a certain condition is met before executing a task.

```c# 
private IEnumerator PlayCutscene ()
{
    input.enabled = false;
    cutscene.Play();

    while (cutscene.IsPlaying)
    {
        yield return null;
    }

    input.enabled = true;     
}
```
> Start playing a cutscene and disable the input until the cutscene has ended.

To do this with STasks, you can use `STasks.DoWhen(action, condition)`

```c#
private void PlayCutscene () 
{
    input.enabled = false;
    cutscene.Play();
    STasks.DoWhen (action: () => input.enabled = true, condition: !cutscene.IsPlaying);
}
```

### Saving an STask
All the methods shown in the previous examples return the STasks they create. You can (and are encouraged to) save these tasks to read values from them or, more importantly, perform additional actions. 

The most common usage for this is manually killing the task. There are cases where a single task needs to be called multiple times. If these tasks overlap, they'll affect the same values at the same time, causing extremely unexpected behaviour. 

Let's take our previous example where we took a life from the player once per second:
```c#
int currentLives = 10;
private void StartTakingLivesFromPlayer () 
{
    STasks.DoUntil(action: () => currentLives--, condition: () => currentLives == 0, every: 1.0f);
}
```
If `StartTakingLivesFromPlayer()` is called again before the task is complete, we'll have two different tasks taking lives from the player at the same time. The player will be sad, confused and frustrated. To avoid this, let's save the task.

```c#
STask takeLivesFromPlayerTask;
int currentLives = 10;

private void StartTakingLivesFromPlayer () 
{
    // If there's already taking lives from the player, stop it.
    takeLivesFromPlayerTask?.Kill();

    // Save the new task
    takeLivesFromPlayerTask = STasks.DoUntil(action: () => currentLives--, condition: () => currentLives == 0, every: 1.0f);
}

```
Alternatively, we could stop a new task from being created if we already have one by checking if it's null.


### STask callbacks

STask provides callbacks to help you execute additional actions when a task is complete or when it's been updated. It's also fluent, meaning you can easily append callbacks one after the other in a single instruction.

#### OnComplete
It's often useful to know when a task has finished. In a previous example, we took a life from our player every second until they're dead. It would be nice if we showed a "Game Over" screen when that happens.
```c#
int currentLives = 10;
STasks.DoUntil(action: () => currentLives--, condition: () => currentLives == 0, every: 1.0f)
    .OnComplete(() => gameOverScreen.Show());
```

#### OnUpdate
We might want to do something else every time the task is updated. 
Let's add a cooldown to one of our player's abilities and update a UI image's fill amount until it's usable again.

```c#
public Image cooldownImage;

private float abilityCooldown = 3.0f;
private bool isUsable = true;

private void ExecuteAbility ()
{
    // Execute our ability
    Debug.Log("Printing to the console is not a real ability, but makes for a good example.");
    
    // Disable the ability and start the cooldown
    isUsable = false;
    STask cooldownTask = STasks.Do(() => isUsable = true, after: abilityCooldown);
    
    // Update the fill amount of our image using the Progress property of our task
    cooldownTask.OnUpdate(() => cooldownImage.fillAmount = cooldownTask.Progress);
}
```

## Contributing
Pull requests are welcome, encouraged and begged for. I created STasks to warm up before a game jam.

## License
This project uses the MIT license. TLDR: feel free to use, copy, modify and redistribute STasks. Creditting me would be really, really nice but not a requirement. If you use STasks for bad things, it's your fault, not mine. Sending me a key to your game is greatly appreciated but the license says nothing about that.

[Full details here](https://github.com/MartinIsla/stasks/blob/master/LICENSE).
