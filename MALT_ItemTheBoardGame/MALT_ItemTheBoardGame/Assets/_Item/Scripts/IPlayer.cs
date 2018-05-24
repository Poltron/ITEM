using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppAdvisory.Item {

	public interface IPlayer  {

		void StartTurn ();
		void EndTurn ();
		void ChangeBallPosition (Cell firstCell, Cell secondCell);
	}
}