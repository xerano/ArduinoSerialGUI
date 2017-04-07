using System;
using Gtk;
using System.Collections.Generic;
using System.IO.Ports;

public partial class MainWindow: Gtk.Window
{

	SerialPort comPort;
	Boolean connected = false;


	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		comPort = new SerialPort();
		string [] availablePorts = SerialPort.GetPortNames ();
		for(int i = 0; i<availablePorts.Length; i++)
		{
			portsComboBox.InsertText (i, availablePorts[i]);
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnPortsComboBoxChanged (object sender, EventArgs e)
	{
		Gtk.ComboBox cb = (Gtk.ComboBox) sender; 
		Console.Out.WriteLine("Combo changed:"+cb.ActiveText);
	}

	protected void OnConnectButtonClicked (object sender, EventArgs e)
	{
		if (!connected) {
			try {
				comPort.PortName = Convert.ToString (portsComboBox.ActiveText);
				comPort.BaudRate = Convert.ToInt32 (baudRateComboBox.ActiveText);
				comPort.DataBits = 8;
				comPort.StopBits = StopBits.One;
				comPort.Handshake = Handshake.None;
				comPort.Parity = Parity.None;
				comPort.Open ();
				connectButton.Label = "Disconnect";
				connected = true;
			} catch (Exception ex) {
				ShowError (ex.Message);
			}
		} else {
			comPort.Close ();
			connectButton.Label = "Connect";
			connected = false;
		}
	}

	private void ShowError(string message) {
		MessageDialog md = new MessageDialog (this, 
			DialogFlags.DestroyWithParent,
			MessageType.Error, 
			ButtonsType.Close, message);

		md.Run ();
		md.Destroy ();
	}

	private void OnDigitalButtonToggle(Gtk.ToggleButton button) {

		if (!comPort.IsOpen) {
			button.Active = false;
			button.Label = "OFF";
			return;
		}

		if (button.Active) {
			button.Label = "ON";
			Gtk.Image onImg = new global::Gtk.Image ();
			onImg.Pixbuf = Stetic.IconLoader.LoadIcon (this, "gtk-yes", global::Gtk.IconSize.Menu);
			button.Image = onImg;

		} else {
			button.Label = "OFF";
			Gtk.Image offImg = new global::Gtk.Image ();
			offImg.Pixbuf = Stetic.IconLoader.LoadIcon (this, "gtk-no", global::Gtk.IconSize.Menu);
			button.Image = offImg;
		}

		int buttonNumber = int.Parse(button.Name.Substring (button.Name.LastIndexOf ('n') + 1));
		if (comPort.IsOpen) {
			if (button.Active) {
				SendSerialCommand ("S"+buttonNumber+"D1");
			} else {
				SendSerialCommand ("S"+buttonNumber+"D0");
			}
		}
	}

	protected void OnToggleButtonPinToggled (object sender, EventArgs e)
	{
		OnDigitalButtonToggle ((Gtk.ToggleButton)sender);
	}

	protected void OnPwmPinSlider3ValueChanged (object sender, EventArgs e)
	{
		Gtk.HScale slider = (Gtk.HScale)sender;
		int sliderNumber = int.Parse (slider.Name.Substring (slider.Name.LastIndexOf ('r') + 1));

		switch (sliderNumber) {
		case 3:
			pwmValue3.Text = "" + (int)slider.Value;
			break;
		case 5:
			pwmValue5.Text = "" + (int)slider.Value;
			break;
		case 6:
			pwmValue6.Text = "" + (int)slider.Value;
			break;
		case 9:
			pwmValue9.Text = "" + (int)slider.Value;
			break;
		case 10:
			pwmValue10.Text = "" + (int)slider.Value;
			break;
		case 11:
			pwmValue11.Text =  "" + (int)slider.Value;
			break;
		default:
			break;
		}

		if (comPort.IsOpen) {
			SendSerialCommand("S"+sliderNumber+"A"+(int) slider.Value);
		}
	}

	private void SendSerialCommand(string command){
		statusCommandEntry.Text = command;
		this.comPort.Write (command + "\r");
	}

	protected void OnPwmEntryValueChanged (object sender, EventArgs e)
	{
		Gtk.Entry entry = (Gtk.Entry)sender;
		int entryNumber = int.Parse (entry.Name.Substring (entry.Name.LastIndexOf ('e') + 1));

		if (entry.Text.Trim ().Length > 0) {
			try {
				double value = double.Parse (entry.Text);
				SetSliderValue(entryNumber, value);
			} catch (Exception ex) {
				ShowError (ex.Message);
			}
		} else {
			SetSliderValue(entryNumber, 0);
		}

	}

	private void SetSliderValue(int sliderNumber, double value){
		switch (sliderNumber) {
		case 3:
			pwmPinSlider3.Value = value;
			break;
		case 5:
			pwmPinSlider5.Value = value;
			break;
		case 6:
			pwmPinSlider6.Value = value;
			break;
		case 9:
			pwmPinSlider9.Value = value;
			break;
		case 10:
			pwmPinSlider10.Value = value;
			break;
		case 11:
			pwmPinSlider11.Value = value;
			break;
		default:
			break;
		}
	}
}
