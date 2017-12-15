function E = applyOffset(E, o)
    % o(1) fix
    S1 = [1 0 0; 0 0 0; 0 0 1];
    d1 = norm(S1*E);
    gamma = -asin(o(1) / d1);
    theta = atan2(E(3), E(1)) - gamma;
    
    % o(2) fix
    S2 = [cos(gamma) 0 0; 0 1 0; 0 0 cos(gamma)];
    d2 = norm(S2*E);
    psi = -asin(o(2) / d2);
    phi = acos(E(2) / d2) - psi;
    
    % o(3) fix
    S3 = cos(psi)*S2;
    d3 = norm(S3*E);
    r = d3 + o(3);

    % spherical to cartesian
    E(1) = r*sin(phi)*cos(theta);
    E(2) = r*cos(phi);
    E(3) = r*sin(theta)*sin(phi);
end

