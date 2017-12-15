function X = createTRSmat(t, r, s)
T = eye(4);
T(1:3, 4) = -t;
R = eye(4);
R(1:3, 1:3) = r;
S = eye(4);
S(1, 1) = s;
S(2, 2) = s;
S(3, 3) = s;
X = R * T * S;
end

