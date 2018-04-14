package md564ef14815cde09ef29d7f8064cedce13;


public class MyTouchListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnTouchListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTouch:(Landroid/view/View;Landroid/view/MotionEvent;)Z:GetOnTouch_Landroid_view_View_Landroid_view_MotionEvent_Handler:Android.Views.View/IOnTouchListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Com.Syncfusion.Numericupdown.MyTouchListener, Syncfusion.SfNumericUpDown.Android, Version=16.1451.0.24, Culture=neutral, PublicKeyToken=null", MyTouchListener.class, __md_methods);
	}


	public MyTouchListener ()
	{
		super ();
		if (getClass () == MyTouchListener.class)
			mono.android.TypeManager.Activate ("Com.Syncfusion.Numericupdown.MyTouchListener, Syncfusion.SfNumericUpDown.Android, Version=16.1451.0.24, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public MyTouchListener (md564ef14815cde09ef29d7f8064cedce13.SfNumericUpDown p0, md564ef14815cde09ef29d7f8064cedce13.NumericTextBox p1, boolean p2)
	{
		super ();
		if (getClass () == MyTouchListener.class)
			mono.android.TypeManager.Activate ("Com.Syncfusion.Numericupdown.MyTouchListener, Syncfusion.SfNumericUpDown.Android, Version=16.1451.0.24, Culture=neutral, PublicKeyToken=null", "Com.Syncfusion.Numericupdown.SfNumericUpDown, Syncfusion.SfNumericUpDown.Android, Version=16.1451.0.24, Culture=neutral, PublicKeyToken=null:Com.Syncfusion.Numericupdown.NumericTextBox, Syncfusion.SfNumericUpDown.Android, Version=16.1451.0.24, Culture=neutral, PublicKeyToken=null:System.Boolean, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public boolean onTouch (android.view.View p0, android.view.MotionEvent p1)
	{
		return n_onTouch (p0, p1);
	}

	private native boolean n_onTouch (android.view.View p0, android.view.MotionEvent p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
