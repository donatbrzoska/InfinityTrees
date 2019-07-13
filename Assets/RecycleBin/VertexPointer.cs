using System;

public class VertexPointer {
	private int size;
	private int offset;

	private int value;

	public VertexPointer(int size, int offset)
	{
		this.size = size;
		this.offset = offset;

		value = offset;
	}

	public void Increment()
	{
		value++;
		//if (value == offset + size)
		//{
		//	value = offset;
		//}
	}

	public void Decrement()
	{
		value--;
		if (value == offset - 1)
		{
			value = offset + size - 1;
		}
	}

	public int Current()
	{
		return value;
	}

	public void Reset()
	{
		value = offset;
	}
}