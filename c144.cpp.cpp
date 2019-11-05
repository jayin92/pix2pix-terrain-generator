#include <bits/stdc++.h>

using namespace std;

int main(){
    string a, b, c, a1, b1, c1;
    cin >> a >> b >> c;
    vector<int> d;
    vector<int> e;
    vector<int> f;
    
    int size = a.size();
    
    for(int i=size-1;i>=0;i--){
        if(a[i] == '?') d.push_back(i);
        if(b[i] == '?') e.push_back(i);
        if(c[i] == '?') f.push_back(i);
    }

    f


}