# KeyHook

A C# library for general hot keys.

## Demo

Reference KeyHook.dll in your code, and then initialize it like so:

	_keyHook = new KeyHook();

Then register your hotkey:

	_keyHook.RegisterHotKey(KeyboardHook.ModifierKeys.Control, Keys.G);

In order to handle your hotkey, you have two options.
You can attach a handler for the specific hotkey like so:

	_keyHook.AttachHandler(new HotKey(KeyboardHook.ModifierKeys.Control, Keys.G), keyHook_G);

	private void keyHook_G(object sender, KeyPressedEventArgs e)
	{
		Debug.WriteLine("G Key Handler");
		Debug.WriteLine("Modifier: {0} Key: {1}",
			e.Modifier, Enum.GetName(typeof(Keys), e.Key));
	}

Or you can handle the keypressed event for all of your hotkeys like so:

	_keyHook.KeyPressed += new EventHandler<KeyPressedEventArgs>(keyHook_KeyPressed);

	private void keyHook_KeyPressed(object sender, KeyPressedEventArgs e)
	{
		Debug.WriteLine("Modifier: {0} Key: {1}",
			e.Modifier, Enum.GetName(typeof(Keys), e.Key));
	}
