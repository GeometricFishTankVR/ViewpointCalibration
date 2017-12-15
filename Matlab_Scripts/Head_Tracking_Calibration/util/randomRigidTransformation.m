function [M, s] = randomRigidTransformation(minScale, maxScale, maxTranslationMagnitude)
if nargin < 3
    maxTranslationMagnitude = 5;
end
if nargin < 2
    maxScale = 1000;
end
if nargin < 1
    minScale = -1000;
end
s = rand(1)*((maxScale - minScale) + minScale);
[r, ~] = qr(randn(3));
t = maxTranslationMagnitude * rand(3,1);

M = createTRSmat(t, r, s);
end