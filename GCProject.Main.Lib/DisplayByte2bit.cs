namespace GCProject.Main.Lib;

public class DisplayByte2bit : NotifyBase
{
	private int _bit7;

	private int _bit6;

	private int _bit5;

	private int _bit4;

	public int Bit7
	{
		get
		{
			return _bit7;
		}
		set
		{
			if (_bit7 != value)
			{
				_bit7 = value;
				OnPropertyChanged("Bit7");
			}
		}
	}

	public int Bit6
	{
		get
		{
			return _bit6;
		}
		set
		{
			if (_bit6 != value)
			{
				_bit6 = value;
				OnPropertyChanged("Bit6");
			}
		}
	}

	public int Bit5
	{
		get
		{
			return _bit5;
		}
		set
		{
			if (_bit5 != value)
			{
				_bit5 = value;
				OnPropertyChanged("Bit5");
			}
		}
	}

	public int Bit4
	{
		get
		{
			return _bit4;
		}
		set
		{
			if (_bit4 != value)
			{
				_bit4 = value;
				OnPropertyChanged("Bit4");
			}
		}
	}

	public byte Value { get; private set; }

	public void SetBits(byte Data)
	{
		Value = Data;
		Bit7 = (((Data & 0x80) == 128) ? 1 : 0);
		Bit6 = (((Data & 0x40) == 64) ? 1 : 0);
		Bit5 = (((Data & 0x20) == 32) ? 1 : 0);
		Bit4 = (((Data & 0x10) == 16) ? 1 : 0);
	}

	public byte UpdateValue()
	{
		int res = 0;
		if (Bit7 > 0)
		{
			res += 128;
		}
		if (Bit6 > 0)
		{
			res += 64;
		}
		if (Bit5 > 0)
		{
			res += 32;
		}
		if (Bit4 > 0)
		{
			res += 16;
		}
		Value = (byte)res;
		return Value;
	}

	public void Clear()
	{
		Bit7 = 0;
		Bit6 = 0;
		Bit5 = 0;
		Bit4 = 0;
		Value = 0;
	}

	public override string ToString()
	{
		return $"bit7:{Bit7} |bit6:{Bit6} |bit5:{Bit5} |bit4:{Bit4}";
	}
}
