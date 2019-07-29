using System;
public class FinishedPointer {
    private int finished = -1;

	public int GetRead() {
		lock (this)
		{
			return finished;
		}
	}

	public int GetWrite() {
		lock (this) {
            if (finished == -1) {
                return 0;
            }

            if (finished==0) {
				return 1;
			} else {
				return 0;
			}
		}
	}

    public void Done() {
        lock (this) {
            if (finished == -1) {
                finished = 0;
            }

            if (finished == 0) {
                finished = 1;
            } else {
                finished = 0;
            }
        }
    }

    public void Reset() {
        finished = -1;
    }
}
