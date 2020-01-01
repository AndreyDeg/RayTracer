namespace WindowsFormsRays.SceneObjects
{
    public static class Utils3D
    {
        public static float Pow8(float d)
        {
            var d2 = d * d;
            var d4 = d2 * d2;
            return d4 * d4;
        }

        public static float min(float l, float r) { return l < r ? l : r; }

        // Rectangle CSG equation. Returns minimum signed distance from 
        // space carved by lowerLeft vertex and opposite rectangle
        // vertex upperRight.
        public static float BoxTest(Vector position, Vector lowerLeft, Vector upperRight)
        {
            lowerLeft = position - lowerLeft;
            upperRight = upperRight - position;
            return -min(
              min(
                min(lowerLeft.x, upperRight.x),
                min(lowerLeft.y, upperRight.y)
              ),
              min(lowerLeft.z, upperRight.z));
        }

		public static float BoxTest2(Vector p, Vector d, Vector lowerLeft, Vector upperRight)
		{
			if (d.x == 0 && d.y == 0 && d.z == 0)
				return BoxTest(p, lowerLeft, upperRight);

			if (p.In(lowerLeft, upperRight))
				return BoxTest(p, lowerLeft, upperRight);

			var minP = lowerLeft - p;
			var maxP = upperRight - p;

			if (minP.x > 0)
			{
				if (d.x > 0)//
				{
					var r = minP.x / d.x;
					if ((d * r).In(minP, maxP)) return r;
				}
			}
			else if (maxP.x < 0)
			{
				if (d.x < 0)
				{
					var r = maxP.x / d.x;
					if ((d * r).In(minP, maxP)) return r;
				}
			}

			if (minP.y > 0)
			{
				if (d.y > 0)
				{
					var r = minP.y / d.y;
					if ((d * r).In(minP, maxP)) return r;
				}
			}
			else if (maxP.y < 0)
			{
				if (d.y < 0)//
				{
					var r = maxP.y / d.y;
					if ((d * r).In(minP, maxP)) return r;
				}
			}

			if (minP.z > 0)
			{
				if (d.z > 0)
				{
					var r = minP.z / d.z;
					if ((d * r).In(minP, maxP)) return r;
				}
			}
			else if (maxP.z < 0)
			{
				if (d.z < 0)//
				{
					var r = maxP.z / d.z;
					if ((d * r).In(minP, maxP)) return r;
				}
			}

			return float.MaxValue;
		}
	}
}
