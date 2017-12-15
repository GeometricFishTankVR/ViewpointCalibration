function q = generateCalibrationPositions(a,b,c,d,n)
%GENERATECALIBRATIONPOSITIONS Summary of this function goes here
% a - minimum radius
% b - maximum radius
% c - minimum angle (in radians) from up direction
% d - maximum angle (in radians) from up direction
spq = n / 4; q = zeros(3,n);
for  i = 1:n
    quadrant = i / spq;
    % Generate restricted random spherical coordinates
    r = (b-a)*rand + a;
    theta = ((mod(i, spq) + 1) / spq) * (pi/6) + pi/6 + (quadrant*pi/2);
    phi =(d-c)*rand + c;
    % Convert to cartesian coordinates
    q(1,i) = r * cos(theta) * sin(phi);
    q(2,i) = r * cos(phi);
    q(3,i) = r * sin(theta) * sin(phi);
end

end

