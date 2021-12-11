void blend_rnm_float(float3 n1, float3 n2, out float3 n_out) {
    n1.z += 1;
    n2.xy = -n2.xy;

    n_out = n1 * dot(n1, n2) / n1.z - n2;
}