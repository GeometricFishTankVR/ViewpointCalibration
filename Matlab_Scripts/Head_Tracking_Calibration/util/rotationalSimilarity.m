function angle = rotationalSimilarity(R1, R2)
E = R1*R2';
d(1) = E(2,3) - E(3,2);
d(2) = E(3,1) - E(1,3);
d(3) = E(1,2) - E(2,1);
angle = asin(norm(d)/2);
end

