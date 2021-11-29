public static class MathUtils {
    public static int mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    public static float remap(float input, float inputMin, float inputMax, float min, float max) {
        return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
    }
}