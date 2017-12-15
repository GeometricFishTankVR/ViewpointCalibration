function [T, center] = findRigidTransformation(p, q, Q, offset,X)

Rotation = findRotation(Q(1:3,:), p(1:3,:), mean(Q(1:3,:), 2), mean(p(1:3,:), 2));

c0 = generalized_center_offset(p(1:3,:), q(1:3,:), Rotation, offset, X);

change = Inf; counter = 1; num_iterations = 12; tol = 1e-12;
while( counter < num_iterations && change > tol)
    Rotation = find_rotation(Q(1:3,:), p(1:3,:), [0;0;0], c0);
    c1 = generalized_center_offset(p(1:3,:), q(1:3,:), Rotation, offset,X);
    change = norm(c1 - c0, Inf);
    c0 = c1;
    counter = counter + 1;
end
center = c0;
Rotation = findRotation(p(1:3,:), Q(1:3,:), center, [0;0;0]);

Translation = -Rotation * center;

T = [Rotation Translation; 0 0 0 1];

