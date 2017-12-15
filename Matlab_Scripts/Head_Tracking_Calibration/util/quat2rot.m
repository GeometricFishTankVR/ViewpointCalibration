function R = quat2rot(q)
% Convert quaternion to rotation matrix.
%
% Input arguments:
% q:
%    the rotation as a quaternion where the last entry is the "scalar term"
%
% Output arguments:
% R:
%    a rotation matrix (i.e. an orthogonal matrix with determinant 1)

% Copyright 2011 Levente Hunyadi

% Dylan Fafard modified this code so that the first entry is the "scalar
% term"

validateattributes(q, {'numeric'}, {'real','vector'});
q = q(:);
validateattributes(q, {'numeric'}, {'size',[4,1]});

w = q(1);
x = q(2);
y = q(3);
z = q(4);

xx = 2*x*x; yy = 2*y*y; zz = 2*z*z;
wx = 2*w*x; wy = 2*w*y; wz = 2*w*z;
xy = 2*x*y; xz = 2*x*z;
yz = 2*y*z;

R = [1 - yy - zz,     xy - wz,     xz + wy;
         xy + wz, 1 - xx - zz,     yz - wx;
         xz - wy,     yz + wx, 1 - xx - yy]; 