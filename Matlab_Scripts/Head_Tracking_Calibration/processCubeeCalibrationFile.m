filename
[p, q, s] = getDataFromCalibrationFile(filename);
p = [p*s; ones(1, size(p, 2))]; % remove the scale
q = [q; ones(1, size(q, 2))];
thresh = 1e-12; maxIter = 600;
n = size(q,2);
X = zeros(4,4,n);
for i = 1:n
    X(:,:,i) = lookAt(q(:,i),[0;0;0],[0;1;0]);
end
[R,O,fval] = minimizeRotationAndOffset(p, q, X, thresh, maxIter);

% calculate residual
r = zeros(1,n);
for i = 1:n
    pdd = applyOffset(R,p(1:3,i),O);
    err2 = X(:,:,i)*[pdd; 1];
    r(i) = norm(err2(1:2));
end

R(1:3,1:3) = R(1:3,1:3) * s % add the scale back in
O(1) = -O(1) % rhs to lhs conversion
setDataInCalibrationFile(filename, R, O)

mean_disp = fval / n
mean_r = mean(r)