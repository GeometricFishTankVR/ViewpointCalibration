function Rx = findRotation(p, q, mp, mq)
N = size(p, 2);
pc = p - repmat(mp,1,N);
qc = q - repmat(mq,1,N);

A = qc * pc';
[U, ~, V] = svd(A);
S = eye(3);
S(3,3) = sign(det(U)*det(V));
Rx = U * S * V';
end