function Ti = inverseRigidTransformation(T)
R = T(1:3, 1:3);
s = (norm(R(:,1)) + norm(R(:,1)) + norm(R(:,2))) / 3;
R = R/s;
t = T(1:3, 4);
Ti = [R'/s -R'*t/s; 0 0 0 1];
end

