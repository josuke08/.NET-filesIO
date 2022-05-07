#include <stdio.h>
#include <stdlib.h>

void main(){
    int n;
    int i = 0, j;
    scanf("%d", &n);
    while(i < n){
        printf("%d\n", ++i);
        j = 1;
        while( j <= 3)
            printf("%d", j++);
        printf("\n");
    }
}
