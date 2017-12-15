function [r_err, t_err] = compareTransformationMatrices(T1, T2)
R1 = T1(1:3,1:3); R2 = T2(1:3,1:3);
s1 = (norm(R1(:,1)) + norm(R1(:,1)) + norm(R1(:,2))) / 3;
s2 = (norm(R2(:,1)) + norm(R2(:,1)) + norm(R2(:,2))) / 3;
r_err = rotationalSimilarity(R1/s1, R2/s2);
% compare the translations
t_err = norm(T1(1:3, 4) - T2(1:3, 4));
end

